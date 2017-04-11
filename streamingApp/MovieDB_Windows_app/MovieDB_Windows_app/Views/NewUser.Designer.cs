namespace MovieDB_Windows_app.Views
{
    partial class NewUser
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
            if (disposing && (components != null))
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
            this.username_TextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.password_TextBox = new System.Windows.Forms.TextBox();
            this.displNameLabel = new System.Windows.Forms.Label();
            this.displayName_TextBox = new System.Windows.Forms.TextBox();
            this.birthdayLabel = new System.Windows.Forms.Label();
            this.emailLabel = new System.Windows.Forms.Label();
            this.email_TextBox = new System.Windows.Forms.TextBox();
            this.userGroupLabel = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.userGroupComboBox = new System.Windows.Forms.ComboBox();
            this.createUserButton = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // username_TextBox
            // 
            this.username_TextBox.Location = new System.Drawing.Point(99, 12);
            this.username_TextBox.Name = "username_TextBox";
            this.username_TextBox.Size = new System.Drawing.Size(200, 20);
            this.username_TextBox.TabIndex = 0;
            this.username_TextBox.Leave += new System.EventHandler(this.TextBox_Leave);
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(38, 15);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(55, 13);
            this.usernameLabel.TabIndex = 1;
            this.usernameLabel.Text = "Username";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(40, 41);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(53, 13);
            this.passwordLabel.TabIndex = 3;
            this.passwordLabel.Text = "Password";
            // 
            // password_TextBox
            // 
            this.password_TextBox.Location = new System.Drawing.Point(99, 38);
            this.password_TextBox.Name = "password_TextBox";
            this.password_TextBox.Size = new System.Drawing.Size(200, 20);
            this.password_TextBox.TabIndex = 2;
            this.password_TextBox.TextChanged += new System.EventHandler(this.TextBox_Leave);
            // 
            // displNameLabel
            // 
            this.displNameLabel.AutoSize = true;
            this.displNameLabel.Location = new System.Drawing.Point(21, 67);
            this.displNameLabel.Name = "displNameLabel";
            this.displNameLabel.Size = new System.Drawing.Size(72, 13);
            this.displNameLabel.TabIndex = 5;
            this.displNameLabel.Text = "Display Name";
            // 
            // displayName_TextBox
            // 
            this.displayName_TextBox.Location = new System.Drawing.Point(99, 64);
            this.displayName_TextBox.Name = "displayName_TextBox";
            this.displayName_TextBox.Size = new System.Drawing.Size(200, 20);
            this.displayName_TextBox.TabIndex = 4;
            this.displayName_TextBox.TextChanged += new System.EventHandler(this.TextBox_Leave);
            // 
            // birthdayLabel
            // 
            this.birthdayLabel.AutoSize = true;
            this.birthdayLabel.Location = new System.Drawing.Point(48, 93);
            this.birthdayLabel.Name = "birthdayLabel";
            this.birthdayLabel.Size = new System.Drawing.Size(45, 13);
            this.birthdayLabel.TabIndex = 7;
            this.birthdayLabel.Text = "Birthday";
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.Location = new System.Drawing.Point(61, 119);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(32, 13);
            this.emailLabel.TabIndex = 9;
            this.emailLabel.Text = "Email";
            // 
            // email_TextBox
            // 
            this.email_TextBox.Location = new System.Drawing.Point(99, 116);
            this.email_TextBox.Name = "email_TextBox";
            this.email_TextBox.Size = new System.Drawing.Size(200, 20);
            this.email_TextBox.TabIndex = 11;
            this.email_TextBox.TextChanged += new System.EventHandler(this.TextBox_Leave);
            // 
            // userGroupLabel
            // 
            this.userGroupLabel.AutoSize = true;
            this.userGroupLabel.Location = new System.Drawing.Point(32, 145);
            this.userGroupLabel.Name = "userGroupLabel";
            this.userGroupLabel.Size = new System.Drawing.Size(61, 13);
            this.userGroupLabel.TabIndex = 11;
            this.userGroupLabel.Text = "User Group";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(99, 90);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 8;
            // 
            // userGroupComboBox
            // 
            this.userGroupComboBox.FormattingEnabled = true;
            this.userGroupComboBox.Location = new System.Drawing.Point(99, 142);
            this.userGroupComboBox.Name = "userGroupComboBox";
            this.userGroupComboBox.Size = new System.Drawing.Size(200, 21);
            this.userGroupComboBox.TabIndex = 13;
            // 
            // createUserButton
            // 
            this.createUserButton.Location = new System.Drawing.Point(99, 180);
            this.createUserButton.Name = "createUserButton";
            this.createUserButton.Size = new System.Drawing.Size(163, 23);
            this.createUserButton.TabIndex = 14;
            this.createUserButton.Text = "Create user";
            this.createUserButton.UseVisualStyleBackColor = true;
            this.createUserButton.Click += new System.EventHandler(this.createUserButton_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // NewUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(351, 232);
            this.Controls.Add(this.createUserButton);
            this.Controls.Add(this.userGroupComboBox);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.userGroupLabel);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.email_TextBox);
            this.Controls.Add(this.birthdayLabel);
            this.Controls.Add(this.displNameLabel);
            this.Controls.Add(this.displayName_TextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.password_TextBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.username_TextBox);
            this.Name = "NewUser";
            this.Text = "NewUser";
            this.Load += new System.EventHandler(this.NewUser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox username_TextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox password_TextBox;
        private System.Windows.Forms.Label displNameLabel;
        private System.Windows.Forms.TextBox displayName_TextBox;
        private System.Windows.Forms.Label birthdayLabel;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.TextBox email_TextBox;
        private System.Windows.Forms.Label userGroupLabel;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.ComboBox userGroupComboBox;
        private System.Windows.Forms.Button createUserButton;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}