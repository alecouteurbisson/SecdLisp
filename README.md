# SECD Lisp compiler
Based on the SECD compiler described in "Functional programming - Application and Implementation" by Peter Henderson

This is a work in progress and currently it will only compile a top level lambda expression
This machine is actually an SECDR VM with an additional Resume stack to permit catching errors

* The SecdVM project is the SECD virtual machine
* The Bootstrap project compiles new compilers
* The SecdLisp project is a test bench