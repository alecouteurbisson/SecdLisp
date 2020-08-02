(letrec compile

  ;; Compile a whole program
  (compile lambda (e)
    (comp e nil (quote ($ap $stop))))

  ;; Compile an expression, e, in environment, n, with continuation, c
  (comp lambda (e n c)
    (cond

      ;; nil, t and numbers don't require quoting
      ((or (int e) (or (null e) (eq t e)))
       (cons (quote $ldc) (cons e c)) )

      ;; other atoms (symbols) are accessed in the environment
      ((atom e)
       (cons (quote $ld) (cons (location e n) c)) )

      ;; Quoted literals
      ((eq (car e) (quote quote))
       (cons (quote $ldc) (cons (car (cdr e)) c)) )

      ;; Fail terminates the last try option
      ((eq (car e) (quote fail))
       (quote ($fail)) )

      ;; Print the top of the S stack
      ((eq (car e) (quote print))
       (comp (car (cdr e)) n (cons (quote $print) c)) )

      ;; Arithmetic
      ((eq (car e) (quote add))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $add) c))) )

      ((eq (car e) (quote sub))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $sub) c))) )

      ((eq (car e) (quote mul))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $mul) c))) )

      ((eq (car e) (quote div))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $div) c))) )

      ((eq (car e) (quote rem))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $rem) c))) )

      ((eq (car e) (quote leq))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $leq) c))) )

      ;; Test numbers and opcodes for equality
      ((eq (car e) (quote eql))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $eql) c))) )

      ;; Test symbols and lists for identity
      ((eq (car e) (quote eq))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $eq) c))) )

      ;; List operations
      ((eq (car e) (quote car))
       (comp (car (cdr e)) n (cons (quote $car) c)) )

      ((eq (car e) (quote cdr))
       (comp (car (cdr e)) n (cons (quote $cdr) c)) )

      ((eq (car e) (quote cons))
       (comp (car (cdr (cdr e))) n (comp (car (cdr e)) n (cons (quote $cons) c))) )

      ;; Atom type testing
      ((eq (car e) (quote atom))
       (comp (car (cdr e)) n (cons (quote $atom) c)) )

      ;; Test for integer atom
      ((eq (car e) (quote int))
       (comp (car (cdr e)) n (cons (quote $int) c)) )

      ;; Logical
      ((or (eq (car e) (quote not)) (eq (car e) (quote null)))
       (comp (car (cdr e)) n (cons (quote $not) c)) )

      ((eq (car e) (quote and))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $and) c))) )

      ((eq (car e) (quote or))
       (comp (car (cdr e)) n (comp (car (cdr (cdr e))) n (cons (quote $or) c))) )

      ;; Conditionals
      ((eq (car e) (quote cond))
       (docond (cdr e) n c) )

      ((eq (car e) (quote if))
       (let (comp (car (cdr e))
                  n
                  (cons (quote $sel) (cons thenpt (cons elsept c))) )
            (thenpt comp (car (cdr (cdr e))) n (quote ($join)))
            (elsept comp (car (cdr (cdr (cdr e)))) n (quote ($join))) ))

      ((eq (car e) (quote try))
       (let (cons (quote $try) (cons thenpt (cons elsept c)))
            (thenpt comp (car (cdr e)) n (quote ($join)))
            (elsept comp (car (cdr (cdr e))) n (quote ($join))) ))

      ;; Environment creation
      ((eq (car e) (quote lambda))
       (let (cons (quote $ldf) (cons body c))
            (body comp (car (cdr (cdr e)))
                       (cons (car (cdr e)) n)
                       (quote ($rtn)) )))

      ((eq (car e) (quote let))
       (let
         (let (complis args n (cons (quote $ldf)
                                    (cons body (cons (quote $ap) c))))
              (body comp (car (cdr e)) m (quote ($rtn))) )
         (m cons (vars (cdr (cdr e))) n)
         (args exprs (cdr (cdr e))) ))

      ((eq (car e) (quote letrec))
       (let
         (let (cons (quote $dum)
                    (complis args m (cons (quote $ldf) (cons body (cons (quote $rap) c)))) )
              (body comp (car (cdr e)) m (quote ($rtn))) )
         (m cons (vars (cdr (cdr e))) n)
         (args exprs (cdr (cdr e))) ))

      ;; Compile a list of expressions
      (t
       (complis (cdr e) n (comp (car e) n (cons (quote $ap) c))) )))

  ;; Compile a list
  (complis lambda (e n c)
    (if (null e)
        (cons (quote $nil) c)
        (complis (cdr e)
                 n
                 (comp (car e) n (cons (quote $cons) c)) )))

  ;; Locate a symbol, e, in environment, n
  (location lambda (e n)
    (letrec
      (if (member e (car n))
          (cons 0 (posn e (car n)))
          (incar (location e (cdr n))) )

      ;; List membership
      (member lambda (e n)
        (cond
          ((null n)
           nil )
          ((eq e (car n))
           t )
          (t
           (member e (cdr n)) )))

    ;; Get index of e in n
    (posn lambda (e n)
      (if (eq e (car n))
          0
          (add  1 (posn e (cdr n))) ))


    ;; Increment the car of an environment index pair
    (incar lambda (l) (cons (add 1 (car l))
                            (cdr l) ))))

  ;; Get variable from binding
  (vars lambda (d)
    (if (null d)
        nil
        (cons (car (car d)) (vars (cdr d)))))

  ;; Get expressions from binding
  (exprs lambda (d)
    (if (null d)
        nil
        (cons (cdr (car d)) (exprs (cdr d)))))

  ;; Recursively process cond
  (docond lambda (e n c)
    (cond
      ((null e)
       (cons (quote $nil) c) )
      ((eq (car (car e)) t)
       (comp (car (cdr (car e))) n c) )
      (t
       (let (comp (car (car e)) n (cons (quote $sel) (cons thenpt (cons elsept c))))
            (thenpt comp (car (cdr (car e))) n (quote ($join)))
            (elsept docond (cdr e) n (quote ($join))) )))))
