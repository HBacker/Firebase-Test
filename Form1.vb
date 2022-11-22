Imports FireSharp.Config
Imports FireSharp.Response
Imports FireSharp.Interfaces
Imports System.ComponentModel 
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography 'MD5
Imports System.Web.Script.Serialization
Public Class Form1
    Dim default_timeout As Integer = 1
     Dim timeout As Integer
    Dim strUsername As String
    Dim strPassword As String
     Dim strRegisteredAt As String

    Dim packetTime As Integer
Private FBConfig As New FirebaseConfig() With
{
.BasePath = "YOUR_FIREBASE_ADDRESS"
}
    Private client As IFirebaseClient

   
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        btnOK.Text = "Gönder"
        txtPassword.PasswordChar = "*"
        Try
           client = New FireSharp.FirebaseClient(FBConfig)
        LOGGER("Client Durumu:: Başlatıldı!")
        Catch ex As Exception
            LOGGER(ex.Message)
        End Try
    End Sub
    Sub getData()
       
    End Sub
    Function LOGGER(message As String)
        LOG.Text = LOG.Text & vbNewLine & message
        Return Nothing
    End Function

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
           
        btnOK.Enabled = False
        txtUsername.Enabled = False
        txtPassword.Enabled = False
        timeout = 3
        timer.Start
       strUsername= txtUsername.Text.ToLower
       strPassword = hasher(txtPassword.Text).ToString
       strRegisteredAt = DateAndTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        
        If txtUsername.Text = "" Or txtUsername.Text.Length < 6 Or txtPassword.Text = "" Or txtPassword.Text.Length < 6 Then
           LOGGER("Kullanıcı Adı veya Şifreniz en az 6 karakter olmalıdır!")
        Else         
            NewUser()
       End If  
    End Sub
   Private Async Function NewUser() as Task
System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        blocker.Enabled = True
        blocker.Visible = True
       ' blocker.Height = 327
       ' blocker.Width = 232
        countPacketTime.Start       
        Await Task.Run(Sub()
        Dim hash As String = hasher(txtUsername.Text & " - " & strRegisteredAt)
        Dim user As New user() With
            {
        .UserID = hash,
        .Username = strUsername,
        .Password = strPassword,
        .RegisteredAt = strRegisteredAt
        }
            Try
                  Dim FBNewUser = client.Set("Users/" & strUsername & ":" & hash, user)
                LOGGER("Yeni Kullanıcı: Veri Gönderildi!:" & " " & user.UserID.ToString)
            Catch ex As Exception
                LOGGER("Yeni Kullanıcı: Veri Gönderilemedi!!:" & " " & user.UserID.ToString & vbNewLine & "[" & ex.Message & "]")
            End Try     
                       End Sub)
        countPacketTime.Stop
        packetTime = 0
         blocker.Enabled = False
        blocker.Visible = False
       ' blocker.Height = 0
        'blocker.Width = 0
        timeout = 1
        End Function





    Function hasher(ByVal x As String)
        Dim md5 As MD5 = New MD5CryptoServiceProvider()
        Dim result As Byte()
        result = md5.ComputeHash(Encoding.ASCII.GetBytes(x))
        Dim output_hbr As New StringBuilder()
        For i As Integer = 0 To result.Length - 1
            output_hbr.Append(result(i).ToString("x2"))
        Next
        Return output_hbr.ToString.TrimEnd()
    End Function
       Public Function ImageToBase64(image As Image) As String
Using ms As New MemoryStream()
' Convert Image to byte[]  
            Dim Format As System.Drawing.Imaging.ImageFormat = Imaging.ImageFormat.Jpeg
            image.Save(ms, Format)
            Dim imageBytes As Byte() = ms.ToArray()

            ' Convert byte[] to Base64 String  
            Dim base64String As String = Convert.ToBase64String(imageBytes)
            Return base64String
        End Using
    End Function

    Public Function Base64ToImage(base64String As String) As Image
        ' Convert Base64 String to byte[]  
        Dim imageBytes As Byte() = Convert.FromBase64String(base64String)
        Dim ms As New MemoryStream(imageBytes, 0, imageBytes.Length)

        ' Convert byte[] to Image  
        ms.Write(imageBytes, 0, imageBytes.Length)
        Dim image__1 As Image = System.Drawing.Image.FromStream(ms, True)
        Return image__1
    End Function

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles  MyBase.Load
    End Sub

    Private Sub timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick       
        btnOK.Text = timeout
        If timeout > 0 Then
        timeout = timeout - 1         
            Else
             btnOK.Enabled = True
            txtUsername.Enabled = True
            txtPassword.Enabled = True
            btnOK.Text = "Gönder"
           timer.Stop
        End If
        
       
    End Sub

    Private Sub countPacketTime_Tick(sender As Object, e As EventArgs) Handles countPacketTime.Tick
        packetTime = packetTime + 1
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) 
       
    End Sub
    Private Async Function getUsers() As Task
      
        Try
              Dim dt As New DataTable()
        dt.Columns.Add("Username")
        dt.Columns.Add("Password")
        dt.Columns.Add("RegisteredAt")
        dt.Columns.Add("UserID")

        dt.Rows.Clear()
              Dim SRRecord = client.Get("Users/")

        Dim myJsonTool As New JavaScriptSerializer


        Dim myDeserializedItems = myJsonTool.Deserialize(Of Dictionary(Of String, user))(SRRecord.Body)
  
        If myDeserializedItems Is Nothing Then
                           LOGGER("getUsers: Veri yok!")
                Else             
            For Each dictItem As KeyValuePair(Of String, user) In myDeserializedItems
                dt.Rows.Add(dictItem.Value.Username, dictItem.Value.Password, dictItem.Value.RegisteredAt, dictItem.Value.UserID)
            Next
         dtTable.DataSource = dt
                        LOGGER("getUsers: Başarılı!")
        End If
           

        Catch ex As Exception
            LOGGER("getUsers: HATA= " & ex.Message)
        End Try
      
                    
          
    End Function

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dtTable.CellContentClick

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        getUsers()
    End Sub
   
End Class
