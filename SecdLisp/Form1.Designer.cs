namespace SecdLisp
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null; 

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if(disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.txtSource = new System.Windows.Forms.TextBox();
      this.txtObject = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtArgs = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtResult = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // timer1
      // 
      this.timer1.Interval = 1;
      // 
      // txtSource
      // 
      this.txtSource.Location = new System.Drawing.Point(12, 28);
      this.txtSource.Multiline = true;
      this.txtSource.Name = "txtSource";
      this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtSource.Size = new System.Drawing.Size(509, 345);
      this.txtSource.TabIndex = 0;
      // 
      // txtObject
      // 
      this.txtObject.Location = new System.Drawing.Point(527, 28);
      this.txtObject.Multiline = true;
      this.txtObject.Name = "txtObject";
      this.txtObject.ReadOnly = true;
      this.txtObject.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtObject.Size = new System.Drawing.Size(293, 344);
      this.txtObject.TabIndex = 1;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 11);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 14);
      this.label1.TabIndex = 2;
      this.label1.Text = "Lisp";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(524, 11);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(49, 14);
      this.label2.TabIndex = 3;
      this.label2.Text = "Object";
      // 
      // txtArgs
      // 
      this.txtArgs.Location = new System.Drawing.Point(72, 379);
      this.txtArgs.Name = "txtArgs";
      this.txtArgs.Size = new System.Drawing.Size(667, 20);
      this.txtArgs.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(17, 382);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(35, 14);
      this.label3.TabIndex = 5;
      this.label3.Text = "Args";
      // 
      // txtResult
      // 
      this.txtResult.Location = new System.Drawing.Point(72, 410);
      this.txtResult.Multiline = true;
      this.txtResult.Name = "txtResult";
      this.txtResult.ReadOnly = true;
      this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtResult.Size = new System.Drawing.Size(667, 201);
      this.txtResult.TabIndex = 6;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 410);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(49, 14);
      this.label4.TabIndex = 7;
      this.label4.Text = "Result";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(745, 410);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 8;
      this.button1.Text = "Compile";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.Compile);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(745, 449);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 9;
      this.button2.Text = "Run";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.RunLisp);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(832, 623);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.txtResult);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txtArgs);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txtObject);
      this.Controls.Add(this.txtSource);
      this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.TextBox txtSource;
    private System.Windows.Forms.TextBox txtObject;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtArgs;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtResult;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
  }
}

