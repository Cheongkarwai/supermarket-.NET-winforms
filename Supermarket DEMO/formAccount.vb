﻿Imports MySql.Data.MySqlClient
Public Class formAccount

    Private conn As MySqlConnection = DatabaseConnection.connect()
    Private cmd As MySqlCommand
    Private Sub topUpBtn_Click(sender As Object, e As EventArgs) 



    End Sub

    Private Sub validateName()

        If (txtName.Text.Length > 0) Then
            Dim hasNum As Boolean = False
            For Each n In txtName.Text
                If Char.IsNumber(n) Then
                    hasNum = True
                    nameWarning.Visible = True
                    Exit For
                Else
                    nameWarning.Visible = False
                End If
            Next
        Else
            nameWarning.Visible = True
        End If



    End Sub

    Private Sub validateEmail()

        If (txtEmail.Text.Length > 0) Then
            If (txtEmail.Text.Trim Like "*@[A-Z]*.*" Or txtEmail.Text.Trim Like "*@[a-z]*.*" Or txtEmail.Text.Trim Like "*@[0-9]*.*" Or txtEmail.Text.Trim Like "*@[a-z]*-[a-z]*.*") And Not txtEmail.Text.Trim Like "@*.*" Then
                emailWarning.Visible = False
            Else
                emailWarning.Visible = True
            End If
        Else
            emailWarning.Visible = True
        End If

    End Sub

    Private Sub validateContact()

        If txtContact.Text.Length > 0 Then
            If txtContact.Text.Substring(0, 3) = "011" Then
                If txtContact.Text Like "###########" Then
                    contactWarning.Visible = False
                Else
                    contactWarning.Visible = True
                End If
            End If

            If txtContact.Text.Substring(0, 3) Like "01[2-9]" Then
                If txtContact.Text Like "##########" Then
                    contactWarning.Visible = False
                Else
                    contactWarning.Visible = True
                End If
            End If

            If Not txtContact.Text.Substring(0, 3) Like "01[1-9]" Then
                contactWarning.Visible = True
            End If
        Else
            contactWarning.Visible = True
        End If
    End Sub

    Private Sub validatePassword()

        If txtPassword.Text.Length > 0 Then
            If txtPassword.Text.Length >= 6 Then
                passwordWarning.Visible = False
            Else
                passwordWarning.Visible = True
            End If
        Else
            passwordWarning.Visible = True
        End If

    End Sub

    Private Sub editProfileBtn_Click(sender As Object, e As EventArgs) Handles editProfileBtn.Click
        confirmBtn.Visible = True
        cancelBtn.Visible = True
        sender.Visible = False

        allowRead(False)

        showInfo(True)


    End Sub

    Public Sub allowRead(val As Boolean)
        txtName.ReadOnly = val
        txtContact.ReadOnly = val
        txtEmail.ReadOnly = val
        txtPassword.ReadOnly = val

    End Sub

    Public Sub showInfo(val As Boolean)
        nameInfo.Visible = val
        emailInfo.Visible = val
        contactInfo.Visible = val
        passwordInfo.Visible = val

    End Sub

    Private Sub confirmBtn_Click(sender As Object, e As EventArgs) Handles confirmBtn.Click

        'getting the customer information
        Dim form1 As Form1 = ParentForm
        Dim customer = CType(form1.userVal, Customer)

        'validating the info
        validateContact()
        validateName()
        validateEmail()
        validatePassword()

        If nameWarning.Visible = False And emailWarning.Visible = False And contactWarning.Visible = False And passwordWarning.Visible = False Then
            editProfileBtn.Visible = True
            sender.Visible = False
            cancelBtn.Visible = False
            showInfo(False)
            allowRead(True)


            Dim result As DialogResult = MessageBox.Show("Are you sure to change your info?", "Edit Information", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then

                'update customer table in db
                Try
                    conn.Open()
                    cmd = New MySqlCommand("UPDATE Customer 
                                            SET name=@name , 
                                            email=@email,
                                            contactNo=@contactNo
                                            WHERE customerID = @customerID", conn)
                    cmd.Parameters.AddWithValue("@name", txtName.Text)
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text)
                    cmd.Parameters.AddWithValue("@contactNo", txtContact.Text)
                    cmd.Parameters.AddWithValue("@customerID", customer.usernameVal)

                    cmd.ExecuteNonQuery()
                    cmd.Parameters.Clear()

                    customer.nameVal = txtName.Text
                    customer.emailVal = txtEmail.Text
                    customer.contactVal = txtContact.Text

                    'update account table in db
                    cmd.Dispose()
                    cmd = New MySqlCommand()
                    cmd.Connection = conn
                    cmd.CommandText = "UPDATE Account
                                        SET password = @password
                                        WHERE accountID = @accountID"
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text)
                    cmd.Parameters.AddWithValue("@accountID", customer.usernameVal)

                    cmd.ExecuteNonQuery()
                    customer.passwordVal = txtPassword.Text


                Catch ex As Exception
                    MessageBox.Show(ex.Message())
                Finally
                    cmd.Dispose()
                    conn.Close()
                End Try

            Else
                editProfileBtn.Visible = True
                confirmBtn.Visible = False
                sender.Visible = False
                showInfo(False)
                allowRead(True)
            End If

        Else
            MessageBox.Show("Please enter valid info")
        End If

    End Sub

    Private Sub cancelBtn_Click(sender As Object, e As EventArgs) Handles cancelBtn.Click
        editProfileBtn.Visible = True
        confirmBtn.Visible = False
        sender.Visible = False
        showInfo(False)
        allowRead(True)
    End Sub

    'show all the customer information
    Private Sub formAccount_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown


        Dim form1 As Form1 = ParentForm
        'instance of the customer in form1
        Dim customer = CType(form1.userVal, Customer)

        'assign all the data to respective textfield
        txtPassword.Text = customer.passwordVal
        txtUsername.Text = customer.usernameVal
        txtContact.Text = customer.contactVal
        txtBalance.Text = customer.balanceVal
        txtEmail.Text = customer.emailVal
        txtName.Text = customer.nameVal
    End Sub

    Private Sub formAccount_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class