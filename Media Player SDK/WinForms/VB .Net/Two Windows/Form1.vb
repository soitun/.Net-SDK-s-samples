' ReSharper disable InconsistentNaming

Imports VisioForge.Types
Imports VisioForge.Controls.UI.WinForms

Public Class Form1

    Dim form2 As Form2

    Private Sub MediaPlayer1_OnLicenseRequired(sender As Object, e As VisioForge.Types.LicenseEventArgs) Handles MediaPlayer1.OnLicenseRequired 

        form2.LogLicensing(e.Message)

    End Sub

    Private Sub btSelectFile_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSelectFile.Click

        If (openFileDialog1.ShowDialog() = DialogResult.OK) Then
            edFilename.Text = openFileDialog1.FileName
        End If

    End Sub

    Private Sub tbTimeline_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbTimeline.Scroll

        If (Convert.ToInt32(timer1.Tag) = 0) Then
            MediaPlayer1.Position_Set_Time(tbTimeline.Value * 1000)
        End If

    End Sub

    Private Sub btStart_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btStart.Click

        MediaPlayer1.FilenamesOrURL.Add(edFilename.Text)
        MediaPlayer1.Audio_PlayAudio = True

        MediaPlayer1.Source_Mode = VFMediaPlayerSource.File_DS

        MediaPlayer1.Audio_OutputDevice = "Default DirectSound Device"

        If (Not MediaPlayer1.Filter_Supported_VMR9) Then
            MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VideoRenderer
        Else
            MediaPlayer1.Video_Renderer.Video_Renderer = VFVideoRenderer.VMR9
        End If

        MediaPlayer1.MultiScreen_Enabled = true
        MediaPlayer1.MultiScreen_Clear()
        MediaPlayer1.MultiScreen_AddScreen(form2.Screen.Handle, form2.Screen.Width, form2.Screen.Height)

        MediaPlayer1.Play()

        MediaPlayer1.Audio_OutputDevice_Balance_Set(0, tbBalance1.Value)
        MediaPlayer1.Audio_OutputDevice_Volume_Set(0, tbVolume1.Value)

        timer1.Enabled = True

    End Sub

    Private Sub btResume_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btResume.Click

        MediaPlayer1.Resume()

    End Sub

    Private Sub btPause_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btPause.Click

        MediaPlayer1.Pause()

    End Sub

    Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btStop.Click

        MediaPlayer1.Stop()
        timer1.Enabled = False
        tbTimeline.Value = 0

        form2.Screen.Invalidate()

    End Sub

    Private Sub btNextFrame_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btNextFrame.Click

        MediaPlayer1.NextFrame()

    End Sub

    Private Sub tbSpeed_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbSpeed.Scroll

        MediaPlayer1.SetSpeed(tbSpeed.Value / 10.0)

    End Sub

    Private Sub tbVolume1_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbVolume1.Scroll

        MediaPlayer1.Audio_OutputDevice_Volume_Set(0, tbVolume1.Value)

    End Sub

    Private Sub tbBalance1_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbBalance1.Scroll

        MediaPlayer1.Audio_OutputDevice_Balance_Set(0, tbBalance1.Value)

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        
        MediaPlayer1.Debug_Dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\VisioForge\\"

        form2 = New Form2

        form2.Text += " (SDK v" + MediaPlayer1.SDK_Version.ToString() + ", " + MediaPlayer1.SDK_State + ")"

        form2.Show()

    End Sub

    Private Sub timer1_Tick(ByVal sender As System.Object, ByVal e As EventArgs) Handles timer1.Tick

        timer1.Tag = 1
        tbTimeline.Maximum = MediaPlayer1.Duration_Time() / 1000

        Dim value As Integer
        value = MediaPlayer1.Position_Get_Time() / 1000
        If ((value > 0) And (value < tbTimeline.Maximum)) Then
            tbTimeline.Value = value
        End If

        lbTime.Text = MediaPlayer.Helpful_SecondsToTimeFormatted(tbTimeline.Value) + "/" + MediaPlayer.Helpful_SecondsToTimeFormatted(tbTimeline.Maximum)

        timer1.Tag = 0

    End Sub

    Private Sub LinkLabel1_LinkClicked_1(sender As System.Object, e As Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "http://www.visioforge.com/video_tutorials")
        Process.Start(startInfo)

    End Sub

    Private Sub MediaPlayer1_OnError(sender As Object, e As ErrorsEventArgs) Handles MediaPlayer1.OnError

        form2.Log(e.Message)

    End Sub

End Class

' ReSharper restore InconsistentNaming