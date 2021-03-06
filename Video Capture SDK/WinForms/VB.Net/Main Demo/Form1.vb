' ReSharper disable InconsistentNaming

Imports System.Globalization
Imports System.Linq
Imports VisioForge.Types
Imports VisioForge.Controls.UI.WinForms
Imports System.Runtime.InteropServices
Imports VisioForge.Types.OutputFormat
Imports VisioForge.Types.Sources
Imports VisioForge.Types.VideoEffects
Imports VisioForge.Types.GPUVideoEffects
Imports VisioForge.Shared
Imports VisioForge.Tools

Public Class Form1

    <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Unicode)>
    Private Shared Function FindWindowByClass(
     ByVal lpClassName As String,
     ByVal zero As IntPtr) As IntPtr
    End Function

    Dim onvifControl As ONVIFControl

    Dim onvifPtzRanges As ONVIFPTZRanges

    Dim onvifPtzX As Double

    Dim onvifPtzY As Double

    Dim onvifPtzZoom As Double

    ' Zoom
    Dim zoom As Double = 1.0

    Dim zoomShiftX As Integer = 0

    Dim zoomShiftY As Integer = 0

    ReadOnly audioChannelMapperItems As List(Of AudioChannelMapperItem) = New List(Of AudioChannelMapperItem)

    Private Sub AddAudioEffects()

        VideoCapture1.Audio_Effects_Clear(-1)

        VideoCapture1.Audio_Effects_Add(-1, VFAudioEffectType.Amplify, cbAudAmplifyEnabled.Checked, -1, -1)
        VideoCapture1.Audio_Effects_Add(-1, VFAudioEffectType.Equalizer, cbAudEqualizerEnabled.Checked, -1, -1)
        VideoCapture1.Audio_Effects_Add(-1, VFAudioEffectType.DynamicAmplify, cbAudDynamicAmplifyEnabled.Checked, -1, -1)
        VideoCapture1.Audio_Effects_Add(-1, VFAudioEffectType.Sound3D, cbAudSound3DEnabled.Checked, -1, -1)
        VideoCapture1.Audio_Effects_Add(-1, VFAudioEffectType.TrueBass, cbAudTrueBassEnabled.Checked, -1, -1)
    End Sub

    Private Sub FillWMVSettings(ByRef wmvOutput As VFWMVOutput)

        Dim s As String

        If rbWMVInternal9.Checked Then

            wmvOutput.Mode = VFWMVMode.InternalProfile

            If cbWMVInternalProfile9.SelectedIndex <> -1 Then
                wmvOutput.Internal_Profile_Name = cbWMVInternalProfile9.Text
            End If

        ElseIf rbWMVInternal8.Checked Then

            wmvOutput.Mode = VFWMVMode.V8SystemProfile

            If (cbWMVInternalProfile8.SelectedIndex <> -1) Then
                wmvOutput.V8ProfileName = cbWMVInternalProfile8.Text
            End If

        ElseIf rbWMVExternal.Checked Then

            wmvOutput.Mode = VFWMVMode.ExternalProfile
            wmvOutput.External_Profile_FileName = edWMVProfile.Text

        Else

            wmvOutput.Mode = VFWMVMode.CustomSettings

            wmvOutput.Custom_Audio_Codec = cbWMVAudioCodec.Text
            wmvOutput.Custom_Audio_Format = cbWMVAudioFormat.Text
            wmvOutput.Custom_Audio_PeakBitrate = Convert.ToInt32(edWMVAudioPeakBitrate.Text)

            s = cbWMVAudioMode.Text
            If s = "CBR" Then
                wmvOutput.Custom_Audio_Mode = VFWMVStreamMode.CBR
            ElseIf s = "VBR" Then
                wmvOutput.Custom_Audio_Mode = VFWMVStreamMode.VBRBitrate
            ElseIf s = "VBR (Peak)" Then
                wmvOutput.Custom_Audio_Mode = VFWMVStreamMode.VBRPeakBitrate
            Else
                wmvOutput.Custom_Audio_Mode = VFWMVStreamMode.VBRQuality
            End If

            wmvOutput.Custom_Audio_StreamPresent = cbWMVAudioEnabled.Checked

            wmvOutput.Custom_Video_Codec = cbWMVVideoCodec.Text
            wmvOutput.Custom_Video_Width = Convert.ToInt32(edWMVWidth.Text)
            wmvOutput.Custom_Video_Height = Convert.ToInt32(edWMVHeight.Text)
            wmvOutput.Custom_Video_SizeSameAsInput = cbWMVSizeSameAsInput.Checked
            wmvOutput.Custom_Video_FrameRate = Convert.ToDouble(edWMVFrameRate.Text)
            wmvOutput.Custom_Video_KeyFrameInterval = Convert.ToByte(edWMVKeyFrameInterval.Text)
            wmvOutput.Custom_Video_Bitrate = Convert.ToInt32(edWMVVideoBitrate.Text)
            wmvOutput.Custom_Video_Quality = Convert.ToByte(edWMVVideoQuality.Text)

            s = cbWMVVideoMode.Text
            If s = "CBR" Then
                wmvOutput.Custom_Video_Mode = VFWMVStreamMode.CBR
            ElseIf s = "VBR" Then
                wmvOutput.Custom_Video_Mode = VFWMVStreamMode.VBRBitrate
            ElseIf s = "VBR (Peak)" Then
                wmvOutput.Custom_Video_Mode = VFWMVStreamMode.VBRPeakBitrate
            Else
                wmvOutput.Custom_Video_Mode = VFWMVStreamMode.VBRQuality
            End If

            If cbWMVTVFormat.Text = "PAL" Then
                wmvOutput.Custom_Video_TVSystem = VFWMVTVSystem.PAL
            ElseIf cbWMVTVFormat.Text = "NTSC" Then
                wmvOutput.Custom_Video_TVSystem = VFWMVTVSystem.NTSC
            Else
                wmvOutput.Custom_Video_TVSystem = VFWMVTVSystem.Other
            End If

            wmvOutput.Custom_Video_StreamPresent = cbWMVVideoEnabled.Checked

            wmvOutput.Custom_Profile_Name = "My_Profile_1"

        End If

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

        Text += " (SDK v" + VideoCapture1.SDK_Version.ToString() + " date " + VideoCapture1.SDK_BuildDate.Date.ToString("d") + ", " + VideoCapture1.SDK_State + ")"

        Tag = 1

        ' set combobox indexes
        cbMode.SelectedIndex = 0
        cbOutputFormat.SelectedIndex = 0
        cbOGGAverage.SelectedIndex = 6
        cbOGGMaximum.SelectedIndex = 8
        cbOGGMinimum.SelectedIndex = 5
        cbResizeMode.SelectedIndex = 0
        cbImageType.SelectedIndex = 1
        cbTextLogoAlign.SelectedIndex = 0
        cbTextLogoAntialiasing.SelectedIndex = 0
        cbTextLogoDrawMode.SelectedIndex = 0
        cbTextLogoEffectrMode.SelectedIndex = 0
        cbTextLogoGradMode.SelectedIndex = 0
        cbTextLogoShapeType.SelectedIndex = 0
        cbMotDetHLColor.SelectedIndex = 1
        cbIPCameraType.SelectedIndex = 2
        cbRotate.SelectedIndex = 0
        cbBarcodeType.SelectedIndex = 0
        cbNetworkStreamingMode.SelectedIndex = 0
        cbFLACBlockSize.SelectedIndex = 4
        cbSpeexBitrateControl.SelectedIndex = 2
        cbSpeexMode.SelectedIndex = 0

        cbDVChannels.SelectedIndex = 1
        cbDVSampleRate.SelectedIndex = 0
        cbBPS.SelectedIndex = 1
        cbChannels.SelectedIndex = 1
        cbSampleRate.SelectedIndex = 0
        cbBPS2.SelectedIndex = 0
        cbChannels2.SelectedIndex = 0
        cbSampleRate2.SelectedIndex = 0
        cbLameCBRBitrate.SelectedIndex = 8
        cbLameVBRMin.SelectedIndex = 8
        cbLameVBRMax.SelectedIndex = 10
        cbLameSampleRate.SelectedIndex = 0

        cbCustomAudioSourceCategory.SelectedIndex = 0
        cbCustomVideoSourceCategory.SelectedIndex = 0

        For Each screen As Screen In Windows.Forms.Screen.AllScreens
            cbScreenCaptureDisplayIndex.Items.Add(screen.DeviceName.Replace("\\.\DISPLAY", String.Empty))
        Next

        cbScreenCaptureDisplayIndex.SelectedIndex = 0

        cbWebMVideoEndUsageMode.SelectedIndex = 0
        edWebMVideoThreadCount.Text = Environment.ProcessorCount.ToString(CultureInfo.InvariantCulture)
        cbWebMVideoEncoder.SelectedIndex = 0
        cbWebMVideoEndUsageMode.SelectedIndex = 0
        cbWebMVideoKeyframeMode.SelectedIndex = 0
        cbWebMVideoQualityMode.SelectedIndex = 0

        cbFFAspectRatio.SelectedIndex = 0
        cbFFAudioBitrate.SelectedIndex = 8
        cbFFAudioChannels.SelectedIndex = 0
        cbFFAudioSampleRate.SelectedIndex = 1
        cbFFConstaint.SelectedIndex = 0
        cbFFOutputFormat.SelectedIndex = 0

        cbFFEXEAspectRatio.SelectedIndex = 0
        cbFFEXEAudioBitrate.SelectedIndex = 8
        cbFFEXEAudioChannels.SelectedIndex = 0
        cbFFEXEAudioSampleRate.SelectedIndex = 0
        cbFFEXEProfile.SelectedIndex = 7
        cbFFEXEH264Mode.SelectedIndex = 0
        cbFFEXEH264Level.SelectedIndex = 0
        cbFFEXEH264Preset.SelectedIndex = 0
        cbFFEXEH264Profile.SelectedIndex = 0
        cbFFEXEVideoConstraint.SelectedIndex = 0

        cbH264Profile.SelectedIndex = 2
        cbH264Level.SelectedIndex = 0
        cbH264RateControl.SelectedIndex = 1
        cbH264MBEncoding.SelectedIndex = 0
        cbAACOutput.SelectedIndex = 0
        cbAACVersion.SelectedIndex = 0
        cbAACObjectType.SelectedIndex = 1
        cbAACBitrate.SelectedIndex = 16
        cbH264PictureType.SelectedIndex = 0
        cbH264TargetUsage.SelectedIndex = 3

        cbNVENCProfile.SelectedIndex = 2
        cbNVENCLevel.SelectedIndex = 0
        cbNVENCRateControl.SelectedIndex = 1

        cbM4AOutput.SelectedIndex = 0
        cbM4AVersion.SelectedIndex = 0
        cbM4AObjectType.SelectedIndex = 1
        cbM4ABitrate.SelectedIndex = 12

        cbDecklinkOutputAnalog.SelectedIndex = 0
        cbDecklinkOutputBlackToDeck.SelectedIndex = 0
        cbDecklinkOutputComponentLevels.SelectedIndex = 0
        cbDecklinkOutputDownConversion.SelectedIndex = 0
        cbDecklinkOutputDualLink.SelectedIndex = 0
        cbDecklinkOutputHDTVPulldown.SelectedIndex = 0
        cbDecklinkOutputNTSC.SelectedIndex = 0
        cbDecklinkOutputSingleField.SelectedIndex = 0

        cbDecklinkSourceInput.SelectedIndex = 0
        cbDecklinkSourceNTSC.SelectedIndex = 0
        cbDecklinkSourceComponentLevels.SelectedIndex = 0
        cbDecklinkSourceTimecode.SelectedIndex = 0

        cbFaceTrackingColorMode.SelectedIndex = 0
        cbFaceTrackingScalingMode.SelectedIndex = 0
        cbFaceTrackingSearchMode.SelectedIndex = 1

        rbMotionDetectionExProcessor.SelectedIndex = 1
        rbMotionDetectionExDetector.SelectedIndex = 1

        Dim genres As List(Of String) = New List(Of String)
        For Each s As String In VideoCapture.Tags_GetDefaultAudioGenres
            genres.Add(s)
        Next

        For Each s As String In VideoCapture.Tags_GetDefaultVideoGenres
            genres.Add(s)
        Next

        genres.Sort()

        For Each genre As String In genres
            cbTagGenre.Items.Add(genre)
        Next

        cbTagGenre.Text = "Rock"

        cbDirect2DRotate.SelectedIndex = 0

        For i As Integer = 0 To VideoCapture1.Video_Codecs.Count - 1
            cbVideoCodecs.Items.Add(VideoCapture1.Video_Codecs.Item(i))
            cbCustomVideoCodecs.Items.Add(VideoCapture1.Video_Codecs.Item(i))
        Next i

        If cbVideoCodecs.Items.Count > 0 Then
            cbVideoCodecs.SelectedIndex = 0
            cbVideoCodecs_SelectedIndexChanged(sender, e)
            cbCustomVideoCodecs.SelectedIndex = 0
            cbCustomVideoCodecs_SelectedIndexChanged(sender, e)
        End If

        VideoCapture1.TVTuner_Read()

        For i As Integer = 0 To VideoCapture1.TVTuner_Devices.Count - 1
            cbTVTuner.Items.Add(VideoCapture1.TVTuner_Devices.Item(i))
        Next i

        If cbTVTuner.Items.Count > 0 Then
            cbTVTuner.SelectedIndex = 0
        End If

        For i As Integer = 0 To VideoCapture1.TVTuner_TVFormats.Count - 1
            cbTVSystem.Items.Add(VideoCapture1.TVTuner_TVFormats.Item(i))
        Next i

        cbTVSystem.SelectedIndex = 0

        For i As Integer = 0 To VideoCapture1.TVTuner_Countries.Count - 1
            cbTVCountry.Items.Add(VideoCapture1.TVTuner_Countries.Item(i))
        Next i

        cbTVCountry.SelectedIndex = 0

        cbTVTuner_SelectedIndexChanged(sender, e)

        For Each info As VideoCaptureDeviceInfo In VideoCapture1.Video_CaptureDevicesInfo
            cbVideoInputDevice.Items.Add(info.Name)
            cbPIPDevice.Items.Add(info.Name)
        Next

        If cbVideoInputDevice.Items.Count > 0 Then
            cbVideoInputDevice.SelectedIndex = 0
            'cbVideoInputDevice_SelectedIndexChanged(sender, e)
            cbPIPDevice.SelectedIndex = 0
            'cbPIPDevice_SelectedIndexChanged(sender, e)
        End If

        For i As Integer = 0 To VideoCapture1.Audio_Codecs.Count - 1
            cbAudioCodecs.Items.Add(VideoCapture1.Audio_Codecs.Item(i))
            cbAudioCodecs2.Items.Add(VideoCapture1.Audio_Codecs.Item(i))
            cbCustomAudioCodecs.Items.Add(VideoCapture1.Audio_Codecs.Item(i))
        Next i

        If cbAudioCodecs.Items.Count > 0 Then
            cbAudioCodecs.SelectedIndex = 0
            'cbAudioCodecs_SelectedIndexChanged(sender, e)
            cbAudioCodecs2.SelectedIndex = 0
            'cbAudioCodecs2_SelectedIndexChanged(sender, e)
            cbCustomAudioCodecs.SelectedIndex = 0
            'cbCustomAudioCodecs_SelectedIndexChanged(sender, e)
        End If

        cbAudioCodecs.Text = "PCM"
        cbAudioCodecs2.Text = "PCM"

        For Each info As AudioCaptureDeviceInfo In VideoCapture1.Audio_CaptureDevicesInfo
            cbAudioInputDevice.Items.Add(info.Name)
            cbAdditionalAudioSource.Items.Add(info.Name)
        Next

        If cbAudioInputDevice.Items.Count > 0 Then
            cbAudioInputDevice.SelectedIndex = 0
            'cbAudioInputDevice_SelectedIndexChanged(sender, e)
            cbAdditionalAudioSource.SelectedIndex = 0
        End If

        For i As Integer = 0 To VideoCapture1.Audio_OutputDevices.Count - 1
            cbAudioOutputDevice.Items.Add(VideoCapture1.Audio_OutputDevices.Item(i))
        Next i

        If cbAudioOutputDevice.Items.Count > 0 Then
            cbAudioOutputDevice.SelectedIndex = 0
            'cbAudioOutputDevice_SelectedIndexChanged(sender, e)
        End If

        Dim devices As List(Of AudioCaptureDeviceInfo) = (From info In VideoCapture1.Audio_CaptureDevicesInfo Where info.Name = cbAudioInputDevice.Text).ToList()
        If devices.Any() Then
            Dim deviceItem = devices.First()
            If Not IsNothing(deviceItem) Then
                Dim lines = deviceItem.Lines
                For Each item As String In lines
                    cbAudioInputLine.Items.Add(item)
                Next
            End If
        End If

        If cbAudioInputLine.Items.Count > 0 Then
            cbAudioInputLine.SelectedIndex = 0
            'cbAudioInputLine_SelectedIndexChanged(sender, e)
        End If

        cbAudioInputSelectedIndexChanged(sender, e)
        cbVideoInputSelectedIndexChanged(sender, e)

        For i As Integer = 0 To VideoCapture1.DirectShow_Filters.Count - 1
            cbCustomDSFilterA.Items.Add(VideoCapture1.DirectShow_Filters.Item(i))
            cbCustomDSFilterV.Items.Add(VideoCapture1.DirectShow_Filters.Item(i))
            cbCustomMuxer.Items.Add(VideoCapture1.DirectShow_Filters.Item(i))
            cbCustomFilewriter.Items.Add(VideoCapture1.DirectShow_Filters.Item(i))
        Next i

        If cbCustomDSFilterA.Items.Count > 0 Then
            cbCustomDSFilterA.SelectedIndex = 0
            'cbCustomDSFilterA_SelectedIndexChanged(sender, e)
            cbCustomDSFilterV.SelectedIndex = 0
            'cbCustomDSFilterV_SelectedIndexChanged(sender, e)
            cbCustomMuxer.SelectedIndex = 0
            'cbCustomMuxer_SelectedIndexChanged(sender, e)
            cbCustomFilewriter.SelectedIndex = 0
            'cbCustomFilewriter_SelectedIndexChanged(sender, e)
        End If

        rbEVR.Enabled = VideoCapture.Filter_Supported_EVR()
        rbVMR9.Enabled = VideoCapture.Filter_Supported_VMR9()

        If Not (rbVMR9.Enabled And rbEVR.Enabled) Then
            rbVR.Checked = True
        ElseIf (rbEVR.Enabled) Then
            rbEVR.Checked = True
        End If

        rbVR_CheckedChanged(sender, e)

        Dim filters As List(Of String)
        filters = VideoCapture1.Special_Filters(VFSpecialFilterType.HardwareVideoEncoder)
        For i As Integer = 0 To filters.Count - 1
            cbMPEGEncoder.Items.Add(filters.Item(i))
        Next i

        If cbMPEGEncoder.Items.Count > 0 Then
            cbMPEGEncoder.SelectedIndex = 0
        End If

        For i As Integer = 0 To VideoCapture1.DirectShow_Filters.Count - 1
            cbFilters.Items.Add(VideoCapture1.DirectShow_Filters.Item(i))
        Next i

        cbMPEGVideoDecoder.Items.Add("(default)")
        cbMPEGAudioDecoder.Items.Add("(default)")

        filters = VideoCapture1.Special_Filters(VFSpecialFilterType.MPEG12VideoDecoder)
        For i As Integer = 0 To filters.Count - 1
            cbMPEGVideoDecoder.Items.Add(filters.Item(i))
        Next i

        filters = VideoCapture1.Special_Filters(VFSpecialFilterType.MPEG1AudioDecoder)
        For i As Integer = 0 To filters.Count - 1
            cbMPEGAudioDecoder.Items.Add(filters.Item(i))
        Next i

        cbMPEGVideoDecoder.SelectedIndex = 0
        cbMPEGAudioDecoder.SelectedIndex = 0

        If cbVideoCodecs.Items.IndexOf("MJPEG Compressor") <> -1 Then
            cbVideoCodecs.SelectedIndex = cbVideoCodecs.Items.IndexOf("MJPEG Compressor")
        End If

        If cbAudioCodecs.Items.IndexOf("PCM") <> -1 Then
            cbAudioCodecs.SelectedIndex = cbAudioCodecs.Items.IndexOf("PCM")
        End If

        cbWMVAudioSelectedIndexChanged(sender, e)
        cbWMVVideoSelectedIndexChanged(sender, e)
        cbWMVAudioCodec.SelectedIndex = 0
        cbWMVVideoCodec.SelectedIndex = 0
        cbWMVAudioCodec_SelectedIndexChanged(sender, e)

        If cbWMVAudioFormat.Items.Count > 0 Then
            cbWMVAudioFormat.SelectedIndex = 0
        End If

        For i As Integer = 0 To VideoCapture1.WMV_Internal_Profiles.Count - 1
            cbWMVInternalProfile9.Items.Add(VideoCapture1.WMV_Internal_Profiles.Item(i))
        Next i
        cbWMVInternalProfile9.SelectedIndex = 7

        Dim names As List(Of String) = New List(Of String)

        Dim descs As List(Of String) = New List(Of String)

        VideoCapture1.WMV_V8_Profiles(names, descs)

        For i As Integer = 0 To names.Count - 1
            cbWMVInternalProfile8.Items.Add(names.Item(i))
        Next i

        cbWMVInternalProfile8.SelectedIndex = 0

        'audio effects
        For i As Integer = 0 To VideoCapture1.Audio_Effects_Equalizer_Presets.Count - 1
            cbAudEqualizerPreset.Items.Add(VideoCapture1.Audio_Effects_Equalizer_Presets.Item(i))
        Next i

        edScreenshotsFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\VisioForge\"
        edOutput.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\VisioForge\" + "output.avi"

        cbPIPMode.SelectedIndex = 0

        ' BDA
        For Each source As String In VideoCapture.BDA_Sources()

            cbBDASourceDevice.Items.Add(source)

        Next

        For Each receiver As String In VideoCapture.BDA_Receivers()

            cbBDAReceiver.Items.Add(receiver)

        Next

        If (cbBDASourceDevice.Items.Count > 0) Then

            cbBDASourceDevice.SelectedIndex = 0

        End If

        If (cbBDAReceiver.Items.Count > 1) Then

            cbBDAReceiver.SelectedIndex = 1

        End If

        cbBDADeviceStandard.SelectedIndex = 0
        cbDVBSPolarisation.SelectedIndex = 0
        cbDVBCModulation.SelectedIndex = 4

        ' Decklink
        For Each device As DecklinkDeviceInfo In VideoCapture1.Decklink_CaptureDevices

            cbDecklinkCaptureDevice.Items.Add(device.Name)
        Next

        If (cbDecklinkCaptureDevice.Items.Count > 0) Then

            cbDecklinkCaptureDevice.SelectedIndex = 0
            cbDecklinkCaptureDevice_SelectedIndexChanged(Nothing, Nothing)

        End If

    End Sub


    Private Sub cbVideoInputDevice_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbVideoInputDevice.SelectedIndexChanged

        Dim min As Integer = 0
        Dim max As Integer = 0
        Dim k As Integer
        Dim default_value As Integer = 0
        Dim step1 As Integer = 0

        Dim auto1 As Boolean = False

        If cbVideoInputDevice.SelectedIndex <> -1 Then

            VideoCapture1.Video_CaptureDevice = cbVideoInputDevice.Text

            cbVideoInputFormat.Items.Clear()

            Dim deviceItem = (From info In VideoCapture1.Video_CaptureDevicesInfo Where info.Name = cbVideoInputDevice.Text)?.First()
            If Not IsNothing(deviceItem) Then
                Dim formats = deviceItem.VideoFormats
                For Each item As String In formats
                    cbVideoInputFormat.Items.Add(item)
                Next

                If cbVideoInputFormat.Items.Count > 0 Then
                    cbVideoInputFormat.SelectedIndex = 0
                End If

                cbVideoInputSelectedIndexChanged(sender, e)

                cbFramerate.Items.Clear()

                Dim frameRate = deviceItem.VideoFrameRates
                For Each item As String In frameRate
                    cbFramerate.Items.Add(item)
                Next

                If cbFramerate.Items.Count > 0 Then
                    cbFramerate.SelectedIndex = 0
                End If

                'currently device active, we can read TV Tuner name
                Dim tvTuner = deviceItem.TVTuner
                If tvTuner <> "" Then

                    k = cbTVTuner.Items.IndexOf(tvTuner)
                    If k >= 0 Then
                        cbTVTuner.SelectedIndex = k
                    End If

                End If

                cbCrossBarAvailable.Enabled = VideoCapture1.Video_CaptureDevice_CrossBar_Init(cbVideoInputDevice.Text)
                cbCrossBarAvailable.Checked = cbCrossBarAvailable.Enabled

                If cbCrossBarAvailable.Enabled Then

                    cbCrossbarInput.Enabled = True
                    cbCrossbarOutput.Enabled = True
                    rbCrossbarSimple.Enabled = True
                    rbCrossbarAdvanced.Enabled = True
                    cbCrossbarVideoInput.Enabled = True
                    btConnect.Enabled = True
                    cbConnectRelated.Enabled = True
                    lbRotes.Enabled = True

                    VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections()

                    cbCrossbarVideoInput.Items.Clear()

                    Dim inputs As List(Of String)

                    inputs = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput("Video Decoder")
                    For i As Integer = 0 To inputs.Count - 1
                        cbCrossbarVideoInput.Items.Add(inputs.Item(i))
                    Next i

                    If cbCrossbarVideoInput.Items.Count > 0 Then
                        cbCrossbarVideoInput.SelectedIndex = 0
                    End If

                    cbCrossbarOutput.Items.Clear()

                    For i As Integer = 0 To VideoCapture1.Video_CaptureDevice_CrossBar_Outputs.Count - 1
                        cbCrossbarOutput.Items.Add(VideoCapture1.Video_CaptureDevice_CrossBar_Outputs.Item(i))
                    Next i

                    If cbCrossbarOutput.Items.Count > 0 Then
                        cbCrossbarOutput.SelectedIndex = 0
                        cbCrossbarOutput_SelectedIndexChanged(sender, e)
                    End If

                    lbRotes.Items.Clear()
                    For i As Integer = 0 To cbCrossbarOutput.Items.Count - 1

                        Dim input1 As String
                        input1 = VideoCapture1.Video_CaptureDevice_CrossBar_GetConnectedInputForOutput(cbCrossbarOutput.Text)

                        If input1 <> "" Then
                            lbRotes.Items.Add("Input: " + input1 + ", Output: " + cbCrossbarOutput.Items.Item(i))
                        End If

                    Next i

                Else
                    cbCrossbarInput.Enabled = False
                    cbCrossbarOutput.Enabled = False
                    rbCrossbarSimple.Enabled = False
                    rbCrossbarAdvanced.Enabled = False
                    cbCrossbarVideoInput.Enabled = False
                    btConnect.Enabled = False
                    cbConnectRelated.Enabled = False
                    lbRotes.Enabled = False
                End If

                'updating adjust settings
                If VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(VFVideoHardwareAdjustment.Brightness, min, max, step1, default_value, auto1) Then

                    tbAdjBrightness.Minimum = min
                    tbAdjBrightness.Maximum = max
                    tbAdjBrightness.SmallChange = step1
                    tbAdjBrightness.Value = default_value
                    cbAdjBrightnessAuto.Checked = auto1
                    lbAdjBrightnessMin.Text = "Min: " + Convert.ToString(min)
                    lbAdjBrightnessMax.Text = "Max: " + Convert.ToString(max)
                    lbAdjBrightnessCurrent.Text = "Current: " + Convert.ToString(default_value)
                End If

                If VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(VFVideoHardwareAdjustment.Hue, min, max, step1, default_value, auto1) Then

                    tbAdjHue.Minimum = min
                    tbAdjHue.Maximum = max
                    tbAdjHue.SmallChange = step1
                    tbAdjHue.Value = default_value
                    cbAdjHueAuto.Checked = auto1
                    lbAdjHueMin.Text = "Min: " + Convert.ToString(min)
                    lbAdjHueMax.Text = "Max: " + Convert.ToString(max)
                    lbAdjHueCurrent.Text = "Current: " + Convert.ToString(default_value)
                End If

                If VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(VFVideoHardwareAdjustment.Saturation, min, max, step1, default_value, auto1) Then

                    tbAdjSaturation.Minimum = min
                    tbAdjSaturation.Maximum = max
                    tbAdjSaturation.SmallChange = step1
                    tbAdjSaturation.Value = default_value
                    cbAdjSaturationAuto.Checked = auto1
                    lbAdjSaturationMin.Text = "Min: " + Convert.ToString(min)
                    lbAdjSaturationMax.Text = "Max: " + Convert.ToString(max)
                    lbAdjSaturationCurrent.Text = "Current: " + Convert.ToString(default_value)
                End If

                If VideoCapture1.Video_CaptureDevice_VideoAdjust_GetRanges(VFVideoHardwareAdjustment.Contrast, min, max, step1, default_value, auto1) Then

                    tbAdjContrast.Minimum = min
                    tbAdjContrast.Maximum = max
                    tbAdjContrast.SmallChange = step1
                    tbAdjContrast.Value = default_value
                    cbAdjContrastAuto.Checked = auto1
                    lbAdjContrastMin.Text = "Min: " + Convert.ToString(min)
                    lbAdjContrastMax.Text = "Max: " + Convert.ToString(max)
                    lbAdjContrastCurrent.Text = "Current: " + Convert.ToString(default_value)
                End If


                cbUseAudioInputFromVideoCaptureDevice.Enabled = deviceItem.AudioOutput
                btVideoCaptureDeviceSettings.Enabled = deviceItem.DialogDefault
            End If
        End If

    End Sub

    Private Sub cbVideoInputSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbVideoInputFormat.SelectedIndexChanged

        VideoCapture1.Video_CaptureDevice_Format = cbVideoInputFormat.Text

    End Sub

    Private Sub cbAudioInputDevice_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudioInputDevice.SelectedIndexChanged

        If cbAudioInputDevice.SelectedIndex <> -1 Then

            VideoCapture1.Audio_CaptureDevice = cbAudioInputDevice.Text

            cbAudioInputFormat.Items.Clear()

            Dim deviceItem = (From info In VideoCapture1.Audio_CaptureDevicesInfo Where info.Name = cbAudioInputDevice.Text)?.First()
            If Not IsNothing(deviceItem) Then
                For Each s As String In deviceItem.Formats
                    cbAudioInputFormat.Items.Add(s)
                Next

                If cbAudioInputFormat.Items.Count > 0 Then
                    cbAudioInputFormat.SelectedIndex = 0

                    cbAudioInputSelectedIndexChanged(sender, e)

                    cbAudioInputLine.Items.Clear()

                    For Each s As String In deviceItem.Lines
                        cbAudioInputLine.Items.Add(s)
                    Next

                    If cbAudioInputLine.Items.Count > 0 Then
                        cbAudioInputLine.SelectedIndex = 0
                    End If

                    cbAudioInputLine_SelectedIndexChanged(sender, e)

                    btAudioInputDeviceSettings.Enabled = deviceItem.DialogDefault
                End If
            End If

        End If

    End Sub

    Private Sub btAudioSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btAudioSettings.Click

        Dim sName As String = cbAudioCodecs.Text

        If VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.Audio_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.Audio_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btSelectOutput_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSelectOutput.Click

        If saveFileDialog1.ShowDialog() = DialogResult.OK Then
            edOutput.Text = saveFileDialog1.FileName
        End If

    End Sub

    Private Sub btSelectWM_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSelectWM.Click

        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            edWMVProfile.Text = openFileDialog1.FileName
        End If

    End Sub

    Private Sub btStart_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btStart.Click

        If (onvifControl IsNot Nothing) Then
            onvifControl.Disconnect()
            onvifControl.Dispose()
            onvifControl = Nothing

            btONVIFConnect.Text = "Connect"
        End If

        zoom = 1.0
        zoomShiftX = 0
        zoomShiftY = 0

        mmLog.Clear()

        If (cbPIPDevices.Items.Count > 0) Then
            If (cbPIPDevices.Items.IndexOf("Main source") = -1) Then
                cbPIPDevices.Items.Insert(0, "Main source")
            End If
        End If

        VideoCapture1.Video_Renderer.Zoom_Ratio = 0
        VideoCapture1.Video_Renderer.Zoom_ShiftX = 0
        VideoCapture1.Video_Renderer.Zoom_ShiftY = 0

        VideoCapture1.Debug_Mode = cbDebugMode.Checked
        VideoCapture1.Debug_Dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\VisioForge\"
        VideoCapture1.VLC_Path = Environment.GetEnvironmentVariable("VFVLCPATH")

        VideoCapture1.Video_Effects_Clear()

        Select Case cbMode.SelectedIndex
            Case 0 : VideoCapture1.Mode = VFVideoCaptureMode.VideoPreview
            Case 1 : VideoCapture1.Mode = VFVideoCaptureMode.VideoCapture
            Case 2 : VideoCapture1.Mode = VFVideoCaptureMode.AudioPreview
            Case 3 : VideoCapture1.Mode = VFVideoCaptureMode.AudioCapture
            Case 4 : VideoCapture1.Mode = VFVideoCaptureMode.ScreenPreview
            Case 5 : VideoCapture1.Mode = VFVideoCaptureMode.ScreenCapture
            Case 6 : VideoCapture1.Mode = VFVideoCaptureMode.IPPreview
            Case 7 : VideoCapture1.Mode = VFVideoCaptureMode.IPCapture
            Case 8 : VideoCapture1.Mode = VFVideoCaptureMode.BDAPreview
            Case 9 : VideoCapture1.Mode = VFVideoCaptureMode.BDACapture
            Case 10 : VideoCapture1.Mode = VFVideoCaptureMode.CustomPreview
            Case 11 : VideoCapture1.Mode = VFVideoCaptureMode.CustomCapture
            Case 12 : VideoCapture1.Mode = VFVideoCaptureMode.DecklinkSourcePreview
            Case 13 : VideoCapture1.Mode = VFVideoCaptureMode.DecklinkSourceCapture
        End Select

        VideoCapture1.Audio_CaptureDevice = cbAudioInputDevice.Text
        VideoCapture1.Audio_OutputDevice = cbAudioOutputDevice.Text
        VideoCapture1.Audio_CaptureDevice_Format = cbAudioInputFormat.Text
        VideoCapture1.Audio_CaptureDevice_Line = cbAudioInputLine.Text
        VideoCapture1.Audio_CaptureDevice_Format_UseBest = cbUseBestAudioInputFormat.Checked

        VideoCapture1.Additional_Audio_CaptureDevice_MixChannels = rbAddAudioStreamsMix.Checked

        If (VideoCapture1.Mode = VFVideoCaptureMode.ScreenCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.ScreenPreview) Then

            Dim settings As ScreenCaptureSourceSettings = SelectScreenSource()
            VideoCapture1.Screen_Capture_Source = settings

            'from screen

        ElseIf VideoCapture1.Mode = VFVideoCaptureMode.IPCapture Or VideoCapture1.Mode = VFVideoCaptureMode.IPPreview Then

            Dim settings As IPCameraSourceSettings = SelectIPCameraSource()
            VideoCapture1.IP_Camera_Source = settings

        ElseIf ((VideoCapture1.Mode = VFVideoCaptureMode.BDACapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.BDAPreview)) Then

            SelectBDASource()

        ElseIf ((VideoCapture1.Mode = VFVideoCaptureMode.CustomCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.CustomPreview)) Then

            SelectCustomSource()

        ElseIf ((VideoCapture1.Mode = VFVideoCaptureMode.DecklinkSourceCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.DecklinkSourcePreview)) Then

            VideoCapture1.Decklink_Source = New DecklinkSourceSettings()
            VideoCapture1.Decklink_Source.Name = cbDecklinkCaptureDevice.Text
            VideoCapture1.Decklink_Source.VideoFormat = cbDecklinkCaptureVideoFormat.Text

        Else

            SelectVideoCaptureSource()

        End If

        If (VideoCapture1.Mode = VFVideoCaptureMode.ScreenCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.VideoCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.AudioCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.IPCapture) Or (VideoCapture1.Mode = VFVideoCaptureMode.BDACapture) Then
            VideoCapture1.Output_Filename = edOutput.Text
        End If

        ' Network streaming
        VideoCapture1.Network_Streaming_Enabled = False

        If cbNetworkStreaming.Checked Then
            ConfigureNetworkStreaming()
        End If

        VideoCapture1.Audio_RecordAudio = cbRecordAudio.Checked
        VideoCapture1.Audio_PlayAudio = cbPlayAudio.Checked

        ' multiscreen
        ConfigureMultiscreen()

        Dim outputFormat = VFVideoCaptureOutputFormat.AVI
        Select Case cbOutputFormat.SelectedIndex
            Case 0 : outputFormat = VFVideoCaptureOutputFormat.AVI
            Case 1 : outputFormat = VFVideoCaptureOutputFormat.MKV
            Case 2
                outputFormat = VFVideoCaptureOutputFormat.WMV

                Dim wmvOutput As VFWMVOutput = New VFWMVOutput
                FillWMVSettings(wmvOutput)
                VideoCapture1.Output_Format = wmvOutput
            Case 3 : outputFormat = VFVideoCaptureOutputFormat.DV
            Case 4 : outputFormat = VFVideoCaptureOutputFormat.PCM_ACM
            Case 5 : outputFormat = VFVideoCaptureOutputFormat.MP3
            Case 6 : outputFormat = VFVideoCaptureOutputFormat.M4A
            Case 7
                outputFormat = VFVideoCaptureOutputFormat.WMA

                Dim wmaOutput As VFWMAOutput = New VFWMAOutput()
                wmaOutput.ProfileFilename = edWMVProfile.Text
                VideoCapture1.Output_Format = wmaOutput
            Case 8 : outputFormat = VFVideoCaptureOutputFormat.FLAC
            Case 9 : outputFormat = VFVideoCaptureOutputFormat.OggVorbis
            Case 10 : outputFormat = VFVideoCaptureOutputFormat.Speex
            Case 11 : outputFormat = VFVideoCaptureOutputFormat.Custom
            Case 12 : outputFormat = VFVideoCaptureOutputFormat.DirectCaptureDV
            Case 13 : outputFormat = VFVideoCaptureOutputFormat.DirectCaptureAVI
            Case 14 : outputFormat = VFVideoCaptureOutputFormat.DirectCaptureMPEG
            Case 15 : outputFormat = VFVideoCaptureOutputFormat.DirectCaptureMKV
            Case 16 : outputFormat = VFVideoCaptureOutputFormat.DirectCaptureMP4_GDCL
            Case 17 : outputFormat = VFVideoCaptureOutputFormat.DirectCaptureMP4_Monogram
            Case 18 : outputFormat = VFVideoCaptureOutputFormat.WebM
            Case 19 : outputFormat = VFVideoCaptureOutputFormat.FFMPEG_DLL
            Case 20 : outputFormat = VFVideoCaptureOutputFormat.FFMPEG_EXE
            Case 21 : outputFormat = VFVideoCaptureOutputFormat.MP4
            Case 22 : outputFormat = VFVideoCaptureOutputFormat.AnimatedGIF
            Case 23 : outputFormat = VFVideoCaptureOutputFormat.Encrypted
        End Select

        If (outputFormat = VFVideoCaptureOutputFormat.AVI) Then

            Dim aviOutput As VFAVIOutput = New VFAVIOutput
            SetAVIOutput(aviOutput)
            VideoCapture1.Output_Format = aviOutput

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.MKV) Then

            Dim mkvOutput As VFMKVOutput = New VFMKVOutput
            SetMKVOutput(mkvOutput)
            VideoCapture1.Output_Format = mkvOutput

        ElseIf outputFormat = VFVideoCaptureOutputFormat.WMA Then

            Dim wmaOutput As VFWMAOutput = New VFWMAOutput()
            wmaOutput.ProfileFilename = edWMVProfile.Text
            VideoCapture1.Output_Format = wmaOutput

        ElseIf outputFormat = VFVideoCaptureOutputFormat.WMV Then

            'nothing, already applied in FillWMVSettings

        ElseIf outputFormat = VFVideoCaptureOutputFormat.DV Then

            Dim dvOutput As VFDVOutput = New VFDVOutput
            SetDVOutput(dvOutput)
            VideoCapture1.Output_Format = dvOutput

        ElseIf outputFormat = VFVideoCaptureOutputFormat.Custom Then

            Dim customOutput As VFCustomOutput = New VFCustomOutput
            SetCustomOutput(customOutput)
            VideoCapture1.Output_Format = customOutput

        ElseIf outputFormat = VFVideoCaptureOutputFormat.DirectCaptureMPEG Then

            If cbMPEGEncoder.SelectedIndex <> -1 Then
                VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name = cbMPEGEncoder.Text
            End If

        ElseIf outputFormat = VFVideoCaptureOutputFormat.PCM_ACM Then

            Dim acmOutput As VFACMOutput = New VFACMOutput
            SetPCMOutput(acmOutput)
            VideoCapture1.Output_Format = acmOutput

        ElseIf outputFormat = VFVideoCaptureOutputFormat.MP3 Then

            Dim mp3Output As VFMP3Output = New VFMP3Output()
            SetMP3Output(mp3Output)
            VideoCapture1.Output_Format = mp3Output

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.FLAC) Then

            Dim flacOutput As VFFLACOutput = New VFFLACOutput()
            SetFLACOutput(flacOutput)
            VideoCapture1.Output_Format = flacOutput

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.Speex) Then

            SetSpeexOutput()

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.OggVorbis) Then

            SetOggOutput()

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.M4A) Then

            Dim m4aOutput As VFM4AOutput = New VFM4AOutput()
            SetM4AOutput(m4aOutput)
            VideoCapture1.Output_Format = m4aOutput

        ElseIf outputFormat = VFVideoCaptureOutputFormat.WebM Then

            Dim webmOutput As VFWebMOutput = New VFWebMOutput
            SetWebMOutput(webmOutput)
            VideoCapture1.Output_Format = webmOutput

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.FFMPEG_DLL) Then

            Dim ffmpegOutput As VFFFMPEGDLLOutput = New VFFFMPEGDLLOutput()
            SetFFMPEGDLLOutput(ffmpegOutput)
            VideoCapture1.Output_Format = ffmpegOutput

        ElseIf (outputFormat = VFVideoCaptureOutputFormat.FFMPEG_EXE) Then

            Dim ffmpegOutput As VFFFMPEGEXEOutput = New VFFFMPEGEXEOutput()
            FillFFMPEGEXESettings(ffmpegOutput)
            VideoCapture1.Output_Format = ffmpegOutput

        ElseIf ((outputFormat = VFVideoCaptureOutputFormat.MP4) Or
            ((outputFormat = VFVideoCaptureOutputFormat.Encrypted) And (rbEncryptedH264SW.Checked)) Or
                    (VideoCapture1.Network_Streaming_Enabled And (VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.RTSP_H264_AAC_SW))) Then

            Dim mp4Output As VFMP4Output = New VFMP4Output()
            SetMP4Output(mp4Output)

            If (outputFormat = VFVideoCaptureOutputFormat.Encrypted) Then

                mp4Output.Encryption = True
                mp4Output.Encryption_Format = VFEncryptionFormat.MP4_H264_SW_AAC

                If (rbEncryptionKeyString.Checked) Then

                    mp4Output.Encryption_KeyType = VFEncryptionKeyType.String
                    mp4Output.Encryption_Key = edEncryptionKeyString.Text

                ElseIf (rbEncryptionKeyFile.Checked) Then

                    mp4Output.Encryption_KeyType = VFEncryptionKeyType.File
                    mp4Output.Encryption_Key = edEncryptionKeyFile.Text

                Else

                    mp4Output.Encryption_KeyType = VFEncryptionKeyType.Binary
                    mp4Output.Encryption_Key = VideoCapture.ConvertHexStringToByteArray(edEncryptionKeyHEX.Text)

                End If

                If (rbEncryptionModeAES128.Checked) Then
                    mp4Output.Encryption_Mode = VFEncryptionMode.v8_AES128
                Else
                    mp4Output.Encryption_Mode = VFEncryptionMode.v9_AES256
                End If
            End If

            VideoCapture1.Output_Format = mp4Output

        ElseIf outputFormat = VFVideoCaptureOutputFormat.AnimatedGIF Then

            Dim gifOutput As VFAnimatedGIFOutput = New VFAnimatedGIFOutput
            SetGIFOutput(gifOutput)
            VideoCapture1.Output_Format = gifOutput

        End If

        'crossbar
        SelectCrossbar()

        'Video effects
        ConfigureVideoEffects()

        ' Barcode detection
        VideoCapture1.Barcode_Reader_Enabled = cbBarcodeDetectionEnabled.Checked
        VideoCapture1.Barcode_Reader_Type = cbBarcodeType.SelectedIndex

        ' Face tracking
        ConfigureFaceTracking()

        ' Chromakey
        ConfigureChromaKey()

        'Object tracking 
        ConfigureMotionDetectionEx()

        ' Virtual camera output
        VideoCapture1.Virtual_Camera_Output_Enabled = cbVirtualCamera.Checked

        ' Decklink output
        ConfigureDecklink()

        ' Video renderer
        ConfigureVideoRenderer()

        ' resize crop rotate
        ConfigureResizeCropRotate()

        ' MPEG / DV decoding
        ConfigureMPEGDVDecoding()

        'motion detection
        If (cbMotDetEnabled.Checked) Then
            btMotDetUpdateSettings_Click(sender, e) 'apply settings
        End If

        'VU Meters
        ConfigureVUMeter()

        ' Audio enhancement
        VideoCapture1.Audio_Enhancer_Enabled = cbAudioEnhancementEnabled.Checked
        If (VideoCapture1.Audio_Enhancer_Enabled) Then

            VideoCapture1.Audio_Enhancer_Normalize(cbAudioNormalize.Checked)
            VideoCapture1.Audio_Enhancer_AutoGain(cbAudioAutoGain.Checked)

            ApplyAudioInputGains()
            ApplyAudioOutputGains()

            VideoCapture1.Audio_Enhancer_Timeshift(tbAudioTimeshift.Value)

        End If

        ' Audio channels mapping
        If (cbAudioChannelMapperEnabled.Checked) Then
            VideoCapture1.Audio_Channel_Mapper = New AudioChannelMapperSettings()
            VideoCapture1.Audio_Channel_Mapper.Routes = audioChannelMapperItems.ToArray()
            VideoCapture1.Audio_Channel_Mapper.OutputChannelsCount = Convert.ToInt32(edAudioChannelMapperOutputChannels.Text)
        Else
            VideoCapture1.Audio_Channel_Mapper = Nothing
        End If

        'Audio processing
        VideoCapture1.Audio_Effects_Enabled = cbAudioEffectsEnabled.Checked
        If VideoCapture1.Audio_Effects_Enabled Then
            AddAudioEffects()
        End If

        ' separate capture
        ConfigureSeparateCapture()

        ' tags
        ConfigureOutputTags()

        ' PIP
        VideoCapture1.PIP_Mode = cbPIPMode.SelectedIndex
        VideoCapture1.PIP_ResizeQuality = cbPIPResizeMode.SelectedIndex

        If (VideoCapture1.PIP_Mode = VFPIPMode.ChromaKey) Then
            Dim chromaKey = New VFPIPChromaKeySettings()
            chromaKey.Color = pnPIPChromaKeyColor.BackColor
            chromaKey.Tolerance1 = tbPIPChromaKeyTolerance1.Value
            chromaKey.Tolerance2 = tbPIPChromaKeyTolerance2.Value

            VideoCapture1.PIP_ChromaKeySettings = chromaKey
        End If

        ' start
        VideoCapture1.Start()

        edNetworkURL.Text = VideoCapture1.Network_Streaming_URL

    End Sub

    Private Sub ConfigureMotionDetectionEx()

        If (cbMotionDetectionEx.Checked) Then
            VideoCapture1.Motion_DetectionEx = New MotionDetectionExSettings()
            VideoCapture1.Motion_DetectionEx.ProcessorType = rbMotionDetectionExProcessor.SelectedIndex
            VideoCapture1.Motion_DetectionEx.DetectorType = rbMotionDetectionExDetector.SelectedIndex
        Else
            VideoCapture1.Motion_DetectionEx = Nothing
        End If

    End Sub

    Private Sub ConfigureOutputTags()

        If cbTagEnabled.Checked Then

            Dim tags As VFFileTags = New VFFileTags
            tags.Title = edTagTitle.Text
            tags.Performers = New String() {edTagArtists.Text}
            tags.Album = edTagAlbum.Text
            tags.Comment = edTagComment.Text
            tags.Track = Convert.ToUInt32(edTagTrackID.Text)
            tags.Copyright = edTagCopyright.Text
            tags.Year = Convert.ToUInt32(edTagYear.Text)
            tags.Composers = New String() {edTagComposers.Text}
            tags.Genres = New String() {cbTagGenre.Text}
            tags.Lyrics = edTagLyrics.Text

            If Not IsNothing(imgTagCover.Image) Then
                tags.Pictures = New Bitmap() {imgTagCover.Image}
            End If

            VideoCapture1.Tags = tags

        End If
    End Sub

    Private Sub ConfigureSeparateCapture()

        VideoCapture1.SeparateCapture_Enabled = cbSeparateCaptureEnabled.Checked
        If (VideoCapture1.SeparateCapture_Enabled) Then
            If (rbSeparateCaptureStartManually.Checked) Then
                VideoCapture1.SeparateCapture_Mode = VFSeparateCaptureMode.Normal
                VideoCapture1.SeparateCapture_AutostartCapture = False
            ElseIf (rbSeparateCaptureSplitByDuration.Checked) Then
                VideoCapture1.SeparateCapture_Mode = VFSeparateCaptureMode.SplitByDuration
                VideoCapture1.SeparateCapture_AutostartCapture = True
                VideoCapture1.SeparateCapture_TimeThreshold = Convert.ToInt32(edSeparateCaptureDuration.Text)
            ElseIf (rbSeparateCaptureSplitBySize.Checked) Then
                VideoCapture1.SeparateCapture_Mode = VFSeparateCaptureMode.SplitByFileSize
                VideoCapture1.SeparateCapture_AutostartCapture = True
                VideoCapture1.SeparateCapture_FileSizeThreshold = Convert.ToInt32(edSeparateCaptureFileSize.Text) * 1024 * 1024
            End If
        End If
    End Sub

    Private Sub ConfigureVUMeter()

        VideoCapture1.Audio_VUMeter_Enabled = cbVUMeter.Checked
        VideoCapture1.Audio_VUMeter_Pro_Enabled = cbVUMeterPro.Checked

        If (VideoCapture1.Audio_VUMeter_Pro_Enabled) Then

            VideoCapture1.Audio_VUMeter_Pro_Volume = tbVUMeterAmplification.Value

            volumeMeter1.Boost = tbVUMeterBoost.Value / 10.0F
            volumeMeter2.Boost = tbVUMeterBoost.Value / 10.0F

            waveformPainter1.Boost = tbVUMeterBoost.Value / 10.0F
            waveformPainter2.Boost = tbVUMeterBoost.Value / 10.0F

        End If
    End Sub

    Private Sub ConfigureMPEGDVDecoding()

        'MPEG decoding (only for tuners with internal HW encoder)
        If cbMPEGVideoDecoder.SelectedIndex < 1 Then
            VideoCapture1.MPEG_Video_Decoder = "" 'default
        Else
            VideoCapture1.MPEG_Video_Decoder = cbMPEGVideoDecoder.Text
        End If

        If cbMPEGAudioDecoder.SelectedIndex < 1 Then
            VideoCapture1.MPEG_Audio_Decoder = "" 'default
        Else
            VideoCapture1.MPEG_Audio_Decoder = cbMPEGAudioDecoder.Text
        End If

        'DV resolution
        If rbDVResFull.Checked Then
            VideoCapture1.DV_Decoder_Video_Resolution = VFDVVideoResolution.Full
        ElseIf rbDVResHalf.Checked Then
            VideoCapture1.DV_Decoder_Video_Resolution = VFDVVideoResolution.Half
        ElseIf rbDVResQuarter.Checked Then
            VideoCapture1.DV_Decoder_Video_Resolution = VFDVVideoResolution.Quarter
        Else
            VideoCapture1.DV_Decoder_Video_Resolution = VFDVVideoResolution.DC
        End If
    End Sub

    Private Sub ConfigureResizeCropRotate()

        VideoCapture1.Video_ResizeOrCrop_Enabled = False

        If cbResize.Checked Then
            VideoCapture1.Video_ResizeOrCrop_Enabled = True

            VideoCapture1.Video_Resize = New VideoResizeSettings()

            VideoCapture1.Video_Resize.Width = Convert.ToInt32(edResizeWidth.Text)
            VideoCapture1.Video_Resize.Height = Convert.ToInt32(edResizeHeight.Text)
            VideoCapture1.Video_Resize.LetterBox = cbResizeLetterbox.Checked

            Select Case cbResizeMode.SelectedIndex
                Case 0 : VideoCapture1.Video_Resize.Mode = VFResizeMode.NearestNeighbor
                Case 1 : VideoCapture1.Video_Resize.Mode = VFResizeMode.Bilinear
                Case 2 : VideoCapture1.Video_Resize.Mode = VFResizeMode.Bicubic
                Case 3 : VideoCapture1.Video_Resize.Mode = VFResizeMode.Lancroz
            End Select
        Else
            VideoCapture1.Video_Resize = Nothing
        End If

        If (cbCrop.Checked) Then

            VideoCapture1.Video_ResizeOrCrop_Enabled = True

            VideoCapture1.Video_Crop = New VideoCropSettings(
                    Convert.ToInt32(edCropLeft.Text),
                    Convert.ToInt32(edCropTop.Text),
                    Convert.ToInt32(edCropRight.Text),
                    Convert.ToInt32(edCropBottom.Text))

        Else

            VideoCapture1.Video_Crop = Nothing

        End If

        Select Case cbRotate.SelectedIndex
            Case 0 : VideoCapture1.Video_Rotation = VFRotateMode.RotateNone
            Case 1 : VideoCapture1.Video_Rotation = VFRotateMode.Rotate90
            Case 2 : VideoCapture1.Video_Rotation = VFRotateMode.Rotate180
            Case 3 : VideoCapture1.Video_Rotation = VFRotateMode.Rotate270
        End Select
    End Sub

    Private Sub ConfigureVideoRenderer()

        If rbVMR9.Checked Then
            VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.VMR9
        ElseIf rbEVR.Checked Then
            VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.EVR
        ElseIf rbVR.Checked Then
            VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.VideoRenderer
        ElseIf (rbDirect2D.Checked) Then
            VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.Direct2D
        Else
            VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.None
        End If

        If (cbStretch.Checked) Then
            VideoCapture1.Video_Renderer.StretchMode = VFVideoRendererStretchMode.Stretch
        Else
            VideoCapture1.Video_Renderer.StretchMode = VFVideoRendererStretchMode.Letterbox
        End If

        VideoCapture1.Video_Renderer.RotationAngle = Convert.ToInt32(cbDirect2DRotate.Text)

        VideoCapture1.Video_Renderer.BackgroundColor = pnVideoRendererBGColor.BackColor
        VideoCapture1.Video_Renderer.Flip_Horizontal = cbScreenFlipHorizontal.Checked
        VideoCapture1.Video_Renderer.Flip_Vertical = cbScreenFlipVertical.Checked
    End Sub

    Private Sub ConfigureDecklink()

        If cbDecklinkOutput.Checked Then
            VideoCapture1.Decklink_Output = New DecklinkOutputSettings()
            VideoCapture1.Decklink_Output.DVEncoding = cbDecklinkDV.Checked
            VideoCapture1.Decklink_Output.AnalogOutputIREUSA = cbDecklinkOutputNTSC.SelectedIndex = 0
            VideoCapture1.Decklink_Output.AnalogOutputSMPTE = cbDecklinkOutputComponentLevels.SelectedIndex = 0
            VideoCapture1.Decklink_Output.BlackToDeckInCapture = cbDecklinkOutputBlackToDeck.SelectedIndex
            VideoCapture1.Decklink_Output.DualLinkOutputMode = cbDecklinkOutputDualLink.SelectedIndex
            VideoCapture1.Decklink_Output.HDTVPulldownOnOutput = cbDecklinkOutputHDTVPulldown.SelectedIndex
            VideoCapture1.Decklink_Output.SingleFieldOutputForSynchronousFrames = cbDecklinkOutputSingleField.SelectedIndex
            VideoCapture1.Decklink_Output.VideoOutputDownConversionMode = cbDecklinkOutputDownConversion.SelectedIndex
            VideoCapture1.Decklink_Output.VideoOutputDownConversionModeAnalogUsed = cbDecklinkOutputDownConversionAnalogOutput.Checked
            VideoCapture1.Decklink_Output.AnalogOutput = cbDecklinkOutputAnalog.SelectedIndex

            VideoCapture1.Decklink_Input = cbDecklinkSourceInput.SelectedIndex
            VideoCapture1.Decklink_Input_SMPTE = cbDecklinkSourceComponentLevels.SelectedIndex = 0
            VideoCapture1.Decklink_Input_IREUSA = cbDecklinkSourceNTSC.SelectedIndex = 0
            VideoCapture1.Decklink_Input_Capture_Timecode_Source = cbDecklinkSourceTimecode.SelectedIndex
        Else
            VideoCapture1.Decklink_Output = Nothing
        End If

    End Sub

    Private Sub ConfigureChromaKey()

        If cbChromaKeyEnabled.Checked Then
            VideoCapture1.ChromaKey = New ChromaKeySettings()
            VideoCapture1.ChromaKey.ContrastHigh = tbChromaKeyContrastHigh.Value
            VideoCapture1.ChromaKey.ContrastLow = tbChromaKeyContrastLow.Value
            VideoCapture1.ChromaKey.ImageFilename = edChromaKeyImage.Text

            If (rbChromaKeyGreen.Checked) Then
                VideoCapture1.ChromaKey.Color = VFChromaColor.Green
            ElseIf (rbChromaKeyBlue.Checked) Then
                VideoCapture1.ChromaKey.Color = VFChromaColor.Blue
            Else
                VideoCapture1.ChromaKey.Color = VFChromaColor.Red
            End If
        Else
            VideoCapture1.ChromaKey = Nothing
        End If
    End Sub

    Private Sub ConfigureFaceTracking()

        If (cbFaceTrackingEnabled.Checked) Then

            VideoCapture1.Face_Tracking = New FaceTrackingSettings()
            VideoCapture1.Face_Tracking.ColorMode = cbFaceTrackingColorMode.SelectedIndex
            VideoCapture1.Face_Tracking.Highlight = cbFaceTrackingCHL.Checked
            VideoCapture1.Face_Tracking.MinimumWindowSize = Int32.Parse(edFaceTrackingMinimumWindowSize.Text)

            ' VideoCapture1.FaceTracking_ScaleFactor = Int32.Parse(edFaceTrackingScaleFactor.Text)
            VideoCapture1.Face_Tracking.ScalingMode = cbFaceTrackingScalingMode.SelectedIndex
            VideoCapture1.Face_Tracking.SearchMode = cbFaceTrackingSearchMode.SelectedIndex

        Else

            VideoCapture1.Face_Tracking = Nothing

        End If

    End Sub

    Private Sub ConfigureVideoEffects()

        VideoCapture1.Video_Effects_Enabled = cbEffects.Checked
        VideoCapture1.Video_Effects_Clear()

        'Deinterlace
        If cbDeinterlace.Checked And VideoCapture1.Mode <> VFVideoCaptureMode.ScreenCapture And VideoCapture1.Mode <> VFVideoCaptureMode.ScreenPreview Then

            If rbDeintBlendEnabled.Checked Then
                Dim blend As IVFVideoEffectDeinterlaceBlend
                Dim effect = VideoCapture1.Video_Effects_Get("DeinterlaceBlend")
                If IsNothing(effect) Then
                    blend = New VFVideoEffectDeinterlaceBlend(True)
                    VideoCapture1.Video_Effects_Add(blend)
                Else
                    blend = effect
                End If

                If (IsNothing(blend)) Then

                    MessageBox.Show("Unable to configure deinterlace blend effect.")
                    Return
                End If

                blend.Threshold1 = Convert.ToInt32(edDeintBlendThreshold1.Text)
                blend.Threshold2 = Convert.ToInt32(edDeintBlendThreshold2.Text)
                blend.Constants1 = Convert.ToInt32(edDeintBlendConstants1.Text) / 10.0
                blend.Constants2 = Convert.ToInt32(edDeintBlendConstants2.Text) / 10.0
            ElseIf (rbDeintCAVTEnabled.Checked) Then
                Dim cavt As IVFVideoEffectDeinterlaceCAVT
                Dim effect = VideoCapture1.Video_Effects_Get("DeinterlaceCAVT")
                If (IsNothing(effect)) Then
                    cavt = New VFVideoEffectDeinterlaceCAVT(rbDeintCAVTEnabled.Checked, Convert.ToInt32(edDeintCAVTThreshold.Text))
                    VideoCapture1.Video_Effects_Add(cavt)
                Else
                    cavt = effect
                End If

                If (IsNothing(cavt)) Then
                    MessageBox.Show("Unable to configure deinterlace CAVT effect.")
                    Return
                End If

                cavt.Threshold = Convert.ToInt32(edDeintCAVTThreshold.Text)
            Else
                Dim triangle As IVFVideoEffectDeinterlaceTriangle
                Dim effect = VideoCapture1.Video_Effects_Get("DeinterlaceTriangle")
                If (IsNothing(effect)) Then
                    triangle = New VFVideoEffectDeinterlaceTriangle(True, Convert.ToByte(edDeintTriangleWeight.Text))
                    VideoCapture1.Video_Effects_Add(triangle)
                Else
                    triangle = effect
                End If

                If (IsNothing(triangle)) Then
                    MessageBox.Show("Unable to configure deinterlace triangle effect.")
                    Return
                End If

                triangle.Weight = Convert.ToByte(edDeintTriangleWeight.Text)
            End If

        End If

        'Denoise
        If cbDenoise.Checked And VideoCapture1.Mode <> VFVideoCaptureMode.ScreenCapture And VideoCapture1.Mode <> VFVideoCaptureMode.ScreenPreview Then

            If (rbDenoiseCAST.Checked) Then
                Dim cast As IVFVideoEffectDenoiseCAST
                Dim effect = VideoCapture1.Video_Effects_Get("DenoiseCAST")
                If (IsNothing(effect)) Then
                    cast = New VFVideoEffectDenoiseCAST(True)
                    VideoCapture1.Video_Effects_Add(cast)
                Else
                    cast = effect
                End If

                If (IsNothing(cast)) Then
                    MessageBox.Show("Unable to configure denoise CAST effect.")
                    Return
                End If
            Else
                Dim mosquito As IVFVideoEffectDenoiseMosquito
                Dim effect = VideoCapture1.Video_Effects_Get("DenoiseMosquito")
                If (IsNothing(effect)) Then
                    mosquito = New VFVideoEffectDenoiseMosquito(True)
                    VideoCapture1.Video_Effects_Add(mosquito)
                Else
                    mosquito = effect
                End If

                If (IsNothing(mosquito)) Then
                    MessageBox.Show("Unable to configure denoise mosquito effect.")
                    Return
                End If
            End If

        End If

        'Other effects
        If tbLightness.Value > 0 Then
            tbLightness_Scroll(Nothing, Nothing)
        End If

        If tbSaturation.Value < 255 Then
            tbSaturation_Scroll(Nothing, Nothing)
        End If

        If tbContrast.Value > 0 Then
            tbContrast_Scroll(Nothing, Nothing)
        End If

        If tbDarkness.Value > 0 Then
            tbDarkness_Scroll(Nothing, Nothing)
        End If

        If cbGreyscale.Checked Then
            cbGreyscale_CheckedChanged(Nothing, Nothing)
        End If

        If cbInvert.Checked Then
            cbInvert_CheckedChanged(Nothing, Nothing)
        End If

        If (cbZoom.Checked) Then
            cbZoom_CheckedChanged(Nothing, Nothing)
        End If

        If (cbPan.Checked) Then
            cbPan_CheckedChanged(Nothing, Nothing)
        End If

        If cbLiveRotation.Checked Then
            cbLiveRotation_CheckedChanged(Nothing, Nothing)
        End If

        If cbImageLogo.Checked Then
            cbImageLogo_CheckedChanged(Nothing, Nothing)
        End If

        If cbTextLogo.Checked Then
            btTextLogoUpdateParams_Click(Nothing, Nothing)
        End If

        If cbFadeInOut.Checked Then
            cbFadeInOut_CheckedChanged(Nothing, Nothing)
        End If
    End Sub

    Private Sub SelectCrossbar()

        If cbCrossBarAvailable.Enabled Then
            If rbCrossbarSimple.Checked Then
                If cbCrossbarVideoInput.SelectedIndex <> -1 Then
                    VideoCapture1.Video_CaptureDevice_CrossBar_ClearConnections()
                    VideoCapture1.Video_CaptureDevice_CrossBar_Connect(cbCrossbarVideoInput.Text, "Video Decoder", True)
                End If
            Else

                'all routes must be already applied
                'you don't need to do anything
            End If
        End If
    End Sub

    Private Sub SetMP4Output(ByRef mp4Output As VFMP4Output)

        Dim tmp = 0

        ' Main settings
        If rbMP4Legacy.Checked Then
            mp4Output.MP4Mode = VFMP4Mode.v8
        ElseIf (rbMP4Modern.Checked) Then
            mp4Output.MP4Mode = VFMP4Mode.v10
        Else
            mp4Output.MP4Mode = VFMP4Mode.NVENC
        End If

        If (mp4Output.MP4Mode <> VFMP4Mode.NVENC) Then
            ' Video H264 settings
            Select Case (cbH264Profile.SelectedIndex)
                Case 0
                    mp4Output.Video_H264.Profile = VFH264Profile.ProfileAuto
                Case 1
                    mp4Output.Video_H264.Profile = VFH264Profile.ProfileBaseline
                Case 2
                    mp4Output.Video_H264.Profile = VFH264Profile.ProfileMain
                Case 3
                    mp4Output.Video_H264.Profile = VFH264Profile.ProfileHigh
                Case 4
                    mp4Output.Video_H264.Profile = VFH264Profile.ProfileHigh10
                Case 5
                    mp4Output.Video_H264.Profile = VFH264Profile.ProfileHigh422
            End Select

            Select Case (cbH264Level.SelectedIndex)
                Case 0
                    mp4Output.Video_H264.Level = VFH264Level.LevelAuto
                Case 1
                    mp4Output.Video_H264.Level = VFH264Level.Level1
                Case 2
                    mp4Output.Video_H264.Level = VFH264Level.Level11
                Case 3
                    mp4Output.Video_H264.Level = VFH264Level.Level12
                Case 4
                    mp4Output.Video_H264.Level = VFH264Level.Level13
                Case 5
                    mp4Output.Video_H264.Level = VFH264Level.Level2
                Case 6
                    mp4Output.Video_H264.Level = VFH264Level.Level21
                Case 7
                    mp4Output.Video_H264.Level = VFH264Level.Level22
                Case 8
                    mp4Output.Video_H264.Level = VFH264Level.Level3
                Case 9
                    mp4Output.Video_H264.Level = VFH264Level.Level31
                Case 10
                    mp4Output.Video_H264.Level = VFH264Level.Level32
                Case 11
                    mp4Output.Video_H264.Level = VFH264Level.Level4
                Case 12
                    mp4Output.Video_H264.Level = VFH264Level.Level41
                Case 13
                    mp4Output.Video_H264.Level = VFH264Level.Level42
                Case 14
                    mp4Output.Video_H264.Level = VFH264Level.Level5
                Case 15
                    mp4Output.Video_H264.Level = VFH264Level.Level51
            End Select

            Select Case (cbH264TargetUsage.SelectedIndex)
                Case 0
                    mp4Output.Video_H264.TargetUsage = VFH264TargetUsage.Auto
                Case 1
                    mp4Output.Video_H264.TargetUsage = VFH264TargetUsage.BestQuality
                Case 2
                    mp4Output.Video_H264.TargetUsage = VFH264TargetUsage.Balanced
                Case 3
                    mp4Output.Video_H264.TargetUsage = VFH264TargetUsage.BestSpeed
            End Select

            Select Case (cbH264PictureType.SelectedIndex)
                Case 0
                    mp4Output.Video_H264.PictureType = VFH264PictureType.Auto
                Case 1
                    mp4Output.Video_H264.PictureType = VFH264PictureType.Frame
                Case 2
                    mp4Output.Video_H264.PictureType = VFH264PictureType.TFF
                Case 3
                    mp4Output.Video_H264.PictureType = VFH264PictureType.BFF
            End Select

            mp4Output.Video_H264.RateControl = cbH264RateControl.SelectedIndex
            mp4Output.Video_H264.MBEncoding = cbH264MBEncoding.SelectedIndex
            mp4Output.Video_H264.GOP = cbH264GOP.Checked
            mp4Output.Video_H264.BitrateAuto = cbH264AutoBitrate.Checked

            Int32.TryParse(edH264IDR.Text, tmp)
            mp4Output.Video_H264.IDR_Period = tmp

            Int32.TryParse(edH264P.Text, tmp)
            mp4Output.Video_H264.P_Period = tmp

            Int32.TryParse(edH264Bitrate.Text, tmp)
            mp4Output.Video_H264.Bitrate = tmp

        Else

            ' NVENC settings
            Select Case (cbNVENCProfile.SelectedIndex)
                Case 0
                    mp4Output.Video_NVENC.Profile = NVENCProfile.Auto
                Case 1
                    mp4Output.Video_NVENC.Profile = NVENCProfile.H264_Baseline
                Case 2
                    mp4Output.Video_NVENC.Profile = NVENCProfile.H264_Main
                Case 3
                    mp4Output.Video_NVENC.Profile = NVENCProfile.H264_High
                Case 4
                    mp4Output.Video_NVENC.Profile = NVENCProfile.H264_High444
                Case 5
                    mp4Output.Video_NVENC.Profile = NVENCProfile.H264_ProgressiveHigh
                Case 6
                    mp4Output.Video_NVENC.Profile = NVENCProfile.H264_ConstrainedHigh
            End Select

            Select Case (cbNVENCLevel.SelectedIndex)
                Case 0
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.Auto
                Case 1
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_1
                Case 2
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_11
                Case 3
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_12
                Case 4
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_13
                Case 5
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_2
                Case 6
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_21
                Case 7
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_22
                Case 8
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_3
                Case 9
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_31
                Case 10
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_32
                Case 11
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_4
                Case 12
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_41
                Case 13
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_42
                Case 14
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_5
                Case 15
                    mp4Output.Video_NVENC.Level = VFNVENCLevel.H264_51
            End Select

            mp4Output.Video_NVENC.Bitrate = Convert.ToInt32(edNVENCBitrate.Text)
            mp4Output.Video_NVENC.QP = Convert.ToInt32(edNVENCQP.Text)
            mp4Output.Video_NVENC.RateControl = cbNVENCRateControl.SelectedIndex
            mp4Output.Video_NVENC.GOP = Convert.ToInt32(edNVENCGOP.Text)
            mp4Output.Video_NVENC.BFrames = Convert.ToInt32(edNVENCBFrames.Text)

        End If

        ' video resize
        If (cbMP4Resize.Checked) Then
            mp4Output.Video_Resize = New VideoResizeSettings()

            mp4Output.Video_Resize.Width = Convert.ToInt32(edMP4ResizeWidth.Text)
            mp4Output.Video_Resize.Height = Convert.ToInt32(edMP4ResizeHeight.Text)
            mp4Output.Video_Resize.LetterBox = cbMP4ResizeLetterbox.Checked

            Select Case (cbResizeMode.SelectedIndex)
                Case 0
                    mp4Output.Video_Resize.Mode = VFResizeMode.NearestNeighbor
                Case 1
                    mp4Output.Video_Resize.Mode = VFResizeMode.Bilinear
                Case 2
                    mp4Output.Video_Resize.Mode = VFResizeMode.Bicubic
                Case 3
                    mp4Output.Video_Resize.Mode = VFResizeMode.Lancroz
            End Select
        End If

        ' Audio AAC settings
        Int32.TryParse(cbAACBitrate.Text, tmp)
        mp4Output.Audio_AAC.Bitrate = tmp

        mp4Output.Audio_AAC.Version = cbAACVersion.SelectedIndex
        mp4Output.Audio_AAC.Output = cbAACOutput.SelectedIndex
        mp4Output.Audio_AAC.Object = (cbAACObjectType.SelectedIndex + 1)

        ' MP4 
        If (cbMP4CustomAVSettings.Checked) Then
            mp4Output.MP4V10Flags = 0

            If (cbMP4TimeAdjust.Checked) Then
                mp4Output.MP4V10Flags = mp4Output.MP4V10Flags Or MP4V10Flags.TimeAdjust
            End If

            If (cbMP4TimeOverride.Checked) Then

                mp4Output.MP4V10Flags = mp4Output.MP4V10Flags Or MP4V10Flags.TimeOverride
            End If
        End If
    End Sub

    Private Sub SetFFMPEGDLLOutput(ByRef ffmpegDLLOutput As VFFFMPEGDLLOutput)

        Select Case (cbFFOutputFormat.SelectedIndex)

            Case 0
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.MPEG1

            Case 1
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.MPEG1VCD

            Case 2
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2

            Case 3
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2SVCD

            Case 4
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2DVD

            Case 5
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.MPEG2TS

            Case 6
                ffmpegDLLOutput.OutputFormat = VFFFMPEGDLLOutputFormat.FLV
        End Select

        Select Case (cbFFAspectRatio.SelectedIndex)

            Case 0
                ffmpegDLLOutput.Video_AspectRatio_W = 0
                ffmpegDLLOutput.Video_AspectRatio_H = 1

            Case 1
                ffmpegDLLOutput.Video_AspectRatio_W = 1
                ffmpegDLLOutput.Video_AspectRatio_H = 1

            Case 2
                ffmpegDLLOutput.Video_AspectRatio_W = 4
                ffmpegDLLOutput.Video_AspectRatio_H = 3

            Case 3
                ffmpegDLLOutput.Video_AspectRatio_W = 16
                ffmpegDLLOutput.Video_AspectRatio_H = 9

        End Select

        Select Case (cbFFConstaint.SelectedIndex)

            Case 0
                ffmpegDLLOutput.Video_TVSystem = VFFFMPEGDLLTVSystem.None

            Case 1
                ffmpegDLLOutput.Video_TVSystem = VFFFMPEGDLLTVSystem.PAL

            Case 2
                ffmpegDLLOutput.Video_TVSystem = VFFFMPEGDLLTVSystem.NTSC

            Case 3
                ffmpegDLLOutput.Video_TVSystem = VFFFMPEGDLLTVSystem.Film

        End Select

        ffmpegDLLOutput.Video_Width = Convert.ToInt32(edFFVideoWidth.Text)
        ffmpegDLLOutput.Video_Height = Convert.ToInt32(edFFVideoHeight.Text)
        ffmpegDLLOutput.Video_Bitrate = Convert.ToInt32(edFFTargetBitrate.Text) * 1000
        ffmpegDLLOutput.Video_MaxBitrate = Convert.ToInt32(edFFVideoBitrateMax.Text) * 1000
        ffmpegDLLOutput.Video_MinBitrate = Convert.ToInt32(edFFVideoBitrateMin.Text) * 1000
        ffmpegDLLOutput.Video_BufferSize = Convert.ToInt32(edFFVBVBufferSize.Text)
        ffmpegDLLOutput.Audio_Channels = Convert.ToInt32(cbFFAudioChannels.Text)
        ffmpegDLLOutput.Audio_SampleRate = Convert.ToInt32(cbFFAudioSampleRate.Text)
        ffmpegDLLOutput.Audio_Bitrate = Convert.ToInt32(cbFFAudioBitrate.Text) * 1000
        ffmpegDLLOutput.Video_Interlace = cbFFVideoInterlace.Checked
    End Sub

    Private Sub SetGIFOutput(ByRef gifOutput As VFAnimatedGIFOutput)

        gifOutput.FrameRate = Convert.ToDouble(edGIFFrameRate.Text)
        gifOutput.ForcedVideoWidth = Convert.ToInt32(edGIFWidth.Text)
        gifOutput.ForcedVideoHeight = Convert.ToInt32(edGIFHeight.Text)

    End Sub

    Private Sub SetWebMOutput(ByRef webmOutput As VFWebMOutput)

        webmOutput.Audio_Quality = tbWebMAudioQuality.Value

        webmOutput.Video_Bitrate = Convert.ToInt32(edWebMVideoBitrate.Text)
        webmOutput.Video_ARNR_MaxFrames = Convert.ToInt32(edWebMVideoARNRMaxFrames.Text)
        webmOutput.Video_ARNR_Strength = Convert.ToInt32(edWebMVideoARNRStrenght.Text)
        webmOutput.Video_ARNR_Type = Convert.ToInt32(edWebMVideoARNRType.Text)
        webmOutput.Video_CPUUsed = Convert.ToInt32(edWebMVideoCPUUsed.Text)
        webmOutput.Video_Decimate = Convert.ToInt32(edWebMVideoDecimate.Text)
        webmOutput.Video_Decoder_Buffer_Size = Convert.ToInt32(edWebMVideoDecoderBufferSize.Text)
        webmOutput.Video_Decoder_Buffer_InitialSize = Convert.ToInt32(edWebMVideoDecoderInitialBuffer.Text)
        webmOutput.Video_Decoder_Buffer_OptimalSize = Convert.ToInt32(edWebMVideoDecoderOptimalBuffer.Text)
        webmOutput.Video_FixedKeyframeInterval = Convert.ToInt32(edWebMVideoFixedKeyframeInterval.Text)
        webmOutput.Video_Keyframe_MaxInterval = Convert.ToInt32(edWebMVideoKeyframeMaxInterval.Text)
        webmOutput.Video_Keyframe_MinInterval = Convert.ToInt32(edWebMVideoKeyframeMinInterval.Text)
        webmOutput.Video_LagInFrames = Convert.ToInt32(edWebMVideoLagInFrames.Text)
        webmOutput.Video_MaxQuantizer = Convert.ToInt32(edWebMVideoMaxQuantizer.Text)
        webmOutput.Video_MinQuantizer = Convert.ToInt32(edWebMVideoMinQuantizer.Text)
        webmOutput.Video_OvershootPct = Convert.ToInt32(edWebMVideoOvershootPct.Text)
        webmOutput.Video_SpatialResampling_DownThreshold = Convert.ToInt32(edWebMVideoSpatialDownThreshold.Text)
        webmOutput.Video_SpatialResampling_UpThreshold = Convert.ToInt32(edWebMVideoSpatialUpThreshold.Text)
        webmOutput.Video_StaticThreshold = Convert.ToInt32(edWebMVideoStaticThreshold.Text)
        webmOutput.Video_ThreadCount = Convert.ToInt32(edWebMVideoThreadCount.Text)
        webmOutput.Video_TokenPartition = Convert.ToInt32(edWebMVideoTokenPartition.Text)
        webmOutput.Video_UndershootPct = Convert.ToInt32(edWebMVideoUndershootPct.Text)
        webmOutput.Video_AutoAltRef = cbWebMVideoAutoAltRef.Checked
        webmOutput.Video_ErrorResilient = cbWebMVideoErrorResilent.Checked
        webmOutput.Video_SpatialResampling_Allowed = cbWebMVideoSpatialResamplingAllowed.Checked
        webmOutput.Video_Encoder = cbWebMVideoEncoder.SelectedIndex

        Select Case cbWebMVideoEndUsageMode.SelectedIndex
            Case 0
                webmOutput.Video_EndUsage = VP8EndUsageMode.Default
            Case 1
                webmOutput.Video_EndUsage = VP8EndUsageMode.CBR
            Case 2
                webmOutput.Video_EndUsage = VP8EndUsageMode.VBR
        End Select

        Select Case cbWebMVideoQualityMode.SelectedIndex
            Case 0
                webmOutput.Video_Mode = VP8QualityMode.Realtime
            Case 1
                webmOutput.Video_Mode = VP8QualityMode.GoodQuality
            Case 2
                webmOutput.Video_Mode = VP8QualityMode.BestQualityBetaDoNotUse
        End Select

        Select Case cbWebMVideoKeyframeMode.SelectedIndex
            Case 0
                webmOutput.Video_Keyframe_Mode = VP8KeyframeMode.Auto
            Case 1
                webmOutput.Video_Keyframe_Mode = VP8KeyframeMode.Default
            Case 2
                webmOutput.Video_Keyframe_Mode = VP8KeyframeMode.Disabled
        End Select
    End Sub

    Private Sub SetM4AOutput(ByRef m4aOutput As VFM4AOutput)

        Dim tmp = 0
        Int32.TryParse(cbM4ABitrate.Text, tmp)
        m4aOutput.Bitrate = tmp

        m4aOutput.Version = cbM4AVersion.SelectedIndex
        m4aOutput.Output = cbM4AOutput.SelectedIndex
        m4aOutput.Object = (cbM4AObjectType.SelectedIndex + 1)
    End Sub

    Private Sub SetOggOutput()

        Dim oggVorbisOutput As VFOGGVorbisOutput = New VFOGGVorbisOutput()

        oggVorbisOutput.Quality = Convert.ToInt32(edOGGQuality.Text)
        oggVorbisOutput.MinBitRate = Convert.ToInt32(cbOGGMinimum.Text)
        oggVorbisOutput.MaxBitRate = Convert.ToInt32(cbOGGMaximum.Text)
        oggVorbisOutput.AvgBitRate = Convert.ToInt32(cbOGGAverage.Text)

        If (rbOGGQuality.Checked) Then
            oggVorbisOutput.Mode = VFVorbisMode.Quality
        Else
            oggVorbisOutput.Mode = VFVorbisMode.Bitrate
        End If

        VideoCapture1.Output_Format = oggVorbisOutput

    End Sub

    Private Sub SetSpeexOutput()

        Dim speexOutput As VFSpeexOutput = New VFSpeexOutput()
        speexOutput.BitRate = tbSpeexBitrate.Value
        speexOutput.BitrateControl = cbSpeexBitrateControl.SelectedIndex
        speexOutput.Mode = cbSpeexMode.SelectedIndex
        speexOutput.MaxBitRate = tbSpeexMaxBitrate.Value
        speexOutput.Complexity = tbSpeexComplexity.Value
        speexOutput.Quality = tbSpeexQuality.Value
        speexOutput.UseAGC = cbSpeexAGC.Checked
        speexOutput.UseDTX = cbSpeexDTX.Checked
        speexOutput.UseDenoise = cbSpeexDenoise.Checked
        speexOutput.UseVAD = cbSpeexVAD.Checked
        VideoCapture1.Output_Format = speexOutput

    End Sub

    Private Sub SetFLACOutput(ByRef flacOutput As VFFLACOutput)

        flacOutput.Level = tbFLACLevel.Value
        flacOutput.BlockSize = Convert.ToInt32(cbFLACBlockSize.Text)
        flacOutput.AdaptiveMidSideCoding = cbFLACAdaptiveMidSideCoding.Checked
        flacOutput.ExhaustiveModelSearch = cbFLACExhaustiveModelSearch.Checked
        flacOutput.LPCOrder = tbFLACLPCOrder.Value
        flacOutput.MidSideCoding = cbFLACMidSideCoding.Checked
        flacOutput.RiceMin = Convert.ToInt32(edFLACRiceMin.Text)
        flacOutput.RiceMax = Convert.ToInt32(edFLACRiceMax.Text)

    End Sub

    Private Sub SetMP3Output(ByRef mp3Output As VFMP3Output)

        'main
        mp3Output.CBR_Bitrate = Convert.ToInt32(cbLameCBRBitrate.Text)
        mp3Output.VBR_MinBitrate = Convert.ToInt32(cbLameVBRMin.Text)
        mp3Output.VBR_MaxBitrate = Convert.ToInt32(cbLameVBRMax.Text)
        mp3Output.SampleRate = Convert.ToInt32(cbLameSampleRate.Text)
        mp3Output.VBR_Quality = tbLameVBRQuality.Value
        mp3Output.EncodingQuality = tbLameEncodingQuality.Value

        If rbLameStandardStereo.Checked Then
            mp3Output.ChannelsMode = VFLameChannelsMode.StandardStereo
        ElseIf rbLameJointStereo.Checked Then
            mp3Output.ChannelsMode = VFLameChannelsMode.JointStereo
        ElseIf rbLameDualChannels.Checked Then
            mp3Output.ChannelsMode = VFLameChannelsMode.DualStereo
        Else
            mp3Output.ChannelsMode = VFLameChannelsMode.Mono
        End If

        mp3Output.VBR_Mode = rbLameVBR.Checked

        'Other
        mp3Output.Copyright = cbLameCopyright.Checked
        mp3Output.Original = cbLameOriginal.Checked
        mp3Output.CRCProtected = cbLameCRCProtected.Checked
        mp3Output.ForceMono = cbLameForceMono.Checked
        mp3Output.StrictlyEnforceVBRMinBitrate = cbLameStrictlyEnforceVBRMinBitrate.Checked
        mp3Output.VoiceEncodingMode = cbLameVoiceEncodingMode.Checked
        mp3Output.KeepAllFrequencies = cbLameKeepAllFrequences.Checked
        mp3Output.StrictISOCompliance = cbLameStrictISOCompilance.Checked
        mp3Output.DisableShortBlocks = cbLameDisableShortBlocks.Checked
        mp3Output.EnableXingVBRTag = cbLameEnableXingVBRTag.Checked
        mp3Output.ModeFixed = cbLameModeFixed.Checked

    End Sub

    Private Sub SetPCMOutput(ByRef acmOutput As VFACMOutput)

        acmOutput.Channels = Convert.ToInt32(cbChannels2.Text)
        acmOutput.BPS = Convert.ToInt32(cbBPS2.Text)
        acmOutput.SampleRate = Convert.ToInt32(cbSampleRate2.Text)

        acmOutput.Name = cbAudioCodecs2.Text

    End Sub

    Private Sub SetCustomOutput(ByRef customOutput As VFCustomOutput)

        If rbCustomUseVideoCodecsCat.Checked Then

            customOutput.Video_Codec = cbCustomVideoCodecs.Text
            customOutput.Video_Codec_UseFiltersCategory = False

        Else

            customOutput.Video_Codec = cbCustomDSFilterV.Text
            customOutput.Video_Codec_UseFiltersCategory = True
        End If

        If rbCustomUseAudioCodecsCat.Checked Then

            customOutput.Audio_Codec = cbCustomAudioCodecs.Text
            customOutput.Audio_Codec_UseFiltersCategory = False

        Else

            customOutput.Audio_Codec = cbCustomDSFilterA.Text
            customOutput.Audio_Codec_UseFiltersCategory = True
        End If

        customOutput.MuxFilter_Name = cbCustomMuxer.Text
        customOutput.MuxFilter_IsEncoder = cbCustomMuxFilterIsEncoder.Checked
        customOutput.SpecialFileWriter_Needed = cbUseSpecialFilewriter.Checked
        customOutput.SpecialFileWriter_FilterName = cbCustomFilewriter.Text
    End Sub

    Private Sub SetDVOutput(ByRef dvOutput As VFDVOutput)

        dvOutput.Audio_Channels = Convert.ToInt32(cbDVChannels.Text)
        dvOutput.Audio_SampleRate = Convert.ToInt32(cbDVSampleRate.Text)

        If rbDVPAL.Checked Then
            dvOutput.Video_Format = VFDVVideoFormat.PAL
        Else
            dvOutput.Video_Format = VFDVVideoFormat.NTSC
        End If

        dvOutput.Type2 = rbDVType2.Checked
    End Sub

    Private Sub SetAVIOutput(ByRef aviOutput As VFAVIOutput)

        aviOutput.ACM.Name = cbAudioCodecs.Text
        aviOutput.ACM.Channels = Convert.ToInt32(cbChannels.Text)
        aviOutput.ACM.BPS = Convert.ToInt32(cbBPS.Text)
        aviOutput.ACM.SampleRate = Convert.ToInt32(cbSampleRate.Text)

        aviOutput.Video_Codec = cbVideoCodecs.Text

        aviOutput.Video_UseCompression = Not cbUncVideo.Checked
        aviOutput.Video_UseCompression_DecodeUncompressedToRGB = cbDecodeToRGB.Checked
        aviOutput.ACM.UseCompression = Not cbUncAudio.Checked

        If cbUseLameInAVI.Checked Then
            aviOutput.Audio_UseMP3Encoder = True
            Dim mp3Output As VFMP3Output = New VFMP3Output
            SetMP3Output(mp3Output)
            aviOutput.MP3 = mp3Output
        End If

    End Sub

    Private Sub SetMKVOutput(ByRef mkvOutput As VFMKVOutput)

        mkvOutput.ACM.Name = cbAudioCodecs.Text
        mkvOutput.ACM.Channels = Convert.ToInt32(cbChannels.Text)
        mkvOutput.ACM.BPS = Convert.ToInt32(cbBPS.Text)
        mkvOutput.ACM.SampleRate = Convert.ToInt32(cbSampleRate.Text)

        mkvOutput.Video_Codec = cbVideoCodecs.Text

        mkvOutput.Video_UseCompression = Not cbUncVideo.Checked
        mkvOutput.Video_UseCompression_DecodeUncompressedToRGB = cbDecodeToRGB.Checked
        mkvOutput.ACM.UseCompression = Not cbUncAudio.Checked

        If cbUseLameInAVI.Checked Then
            mkvOutput.Audio_UseMP3Encoder = True
            Dim mp3Output As VFMP3Output = New VFMP3Output
            SetMP3Output(mp3Output)
            mkvOutput.MP3 = mp3Output
        End If

    End Sub

    Private Sub ConfigureMultiscreen()

        VideoCapture1.MultiScreen_Clear()
        VideoCapture1.MultiScreen_Enabled = cbUseAdditionalScreens.Checked

        If VideoCapture1.MultiScreen_Enabled Then

            VideoCapture1.MultiScreen_AddScreen(pnScreen1.Handle, pnScreen1.Width, pnScreen1.Height)
            VideoCapture1.MultiScreen_AddScreen(pnScreen2.Handle, pnScreen2.Width, pnScreen2.Height)
            VideoCapture1.MultiScreen_AddScreen(pnScreen3.Handle, pnScreen3.Width, pnScreen3.Height)

        End If
    End Sub

    Private Sub ConfigureNetworkStreaming()

        VideoCapture1.Network_Streaming_Enabled = True

        Select Case (cbNetworkStreamingMode.SelectedIndex)

            Case 0
                VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.WMV

                If (rbNetworkStreamingUseMainWMVSettings.Checked) Then

                    Dim wmvOutput As VFWMVOutput = New VFWMVOutput()
                    FillWMVSettings(wmvOutput)
                    VideoCapture1.Network_Streaming_Output = wmvOutput

                Else

                    Dim wmvOutput As VFWMVOutput = New VFWMVOutput()
                    wmvOutput.Mode = VFWMVMode.ExternalProfile
                    wmvOutput.External_Profile_FileName = edNetworkStreamingWMVProfile.Text
                    VideoCapture1.Network_Streaming_Output = wmvOutput

                End If

                VideoCapture1.Network_Streaming_WMV_Maximum_Clients = Convert.ToInt32(edMaximumClients.Text)
                VideoCapture1.Network_Streaming_Network_Port = Convert.ToInt32(edWMVNetworkPort.Text)

            Case 1
                VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.RTSP_H264_AAC_SW

                Dim mp4Output As VFMP4Output = New VFMP4Output()
                SetMP4Output(mp4Output)
                VideoCapture1.Network_Streaming_Output = mp4Output

                VideoCapture1.Network_Streaming_URL = edNetworkRTSPURL.Text

            Case 2

                VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.RTMP_FFMPEG_EXE

                Dim ffmpegOutput As VFFFMPEGEXEOutput = New VFFFMPEGEXEOutput()

                If (rbNetworkUDPFFMPEG.Checked) Then

                    ffmpegOutput.FillDefaults(VFFFMPEGEXEDefaultsProfile.MP4_H264_AAC, True)

                Else

                    FillFFMPEGEXESettings(ffmpegOutput)

                End If

                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.FLV

                VideoCapture1.Network_Streaming_Output = ffmpegOutput
                VideoCapture1.Network_Streaming_URL = edNetworkRTMPURL.Text

            Case 3

                VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.UDP_FFMPEG_EXE

                Dim ffmpegOutput As VFFFMPEGEXEOutput = New VFFFMPEGEXEOutput()

                If (rbNetworkUDPFFMPEG.Checked) Then

                    ffmpegOutput.FillDefaults(VFFFMPEGEXEDefaultsProfile.MP4_H264_AAC, True)

                Else

                    FillFFMPEGEXESettings(ffmpegOutput)

                End If

                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MPEGTS
                VideoCapture1.Network_Streaming_Output = ffmpegOutput

                VideoCapture1.Network_Streaming_URL = edNetworkUDPURL.Text

            Case 4

                If (rbNetworkSSSoftware.Checked) Then

                    VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.SSF_H264_AAC_SW

                    Dim mp4Output As VFMP4Output = New VFMP4Output()
                    SetMP4Output(mp4Output)
                    VideoCapture1.Network_Streaming_Output = mp4Output

                Else

                    VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.SSF_FFMPEG_EXE

                    Dim ffmpegOutput As VFFFMPEGEXEOutput = New VFFFMPEGEXEOutput()

                    If (rbNetworkSSFFMPEGDefault.Checked) Then

                        ffmpegOutput.FillDefaults(VFFFMPEGEXEDefaultsProfile.MP4_H264_AAC, True)

                    Else

                        FillFFMPEGEXESettings(ffmpegOutput)

                    End If

                    ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.ISMV
                    VideoCapture1.Network_Streaming_Output = ffmpegOutput

                End If

                VideoCapture1.Network_Streaming_URL = edNetworkSSURL.Text

            Case 5

                VideoCapture1.Network_Streaming_Format = VFNetworkStreamingFormat.External

        End Select

        VideoCapture1.Network_Streaming_Audio_Enabled = cbNetworkStreamingAudioEnabled.Checked
    End Sub

    Private Sub SelectVideoCaptureSource()

        'from video capture device
        VideoCapture1.Video_CaptureDevice = cbVideoInputDevice.Text
        VideoCapture1.Video_CaptureDevice_IsAudioSource = cbUseAudioInputFromVideoCaptureDevice.Checked
        VideoCapture1.Video_CaptureDevice_Format = cbVideoInputFormat.Text
        VideoCapture1.Video_CaptureDevice_Format_UseBest = cbUseBestVideoInputFormat.Checked

        VideoCapture1.Video_CaptureDevice_UseClosedCaptions = cbUseClosedCaptions.Checked

        If cbFramerate.SelectedIndex <> -1 Then
            VideoCapture1.Video_CaptureDevice_FrameRate = Convert.ToDouble(cbFramerate.Text)
        End If

        VideoCapture1.Video_CaptureDevice_Format_UseBest = cbUseBestVideoInputFormat.Checked

    End Sub

    Private Sub SelectCustomSource()

        VideoCapture1.Custom_Source = New CustomSourceSettings()

        If (cbCustomVideoSourceCategory.SelectedIndex = 0) Then
            VideoCapture1.Custom_Source.VideoFilterCategory = VFFilterCategory.VideoCaptureSource
        Else
            VideoCapture1.Custom_Source.VideoFilterCategory = VFFilterCategory.DirectShowFilters
        End If

        VideoCapture1.Custom_Source.VideoFilterName = cbCustomVideoSourceFilter.Text
        VideoCapture1.Custom_Source.VideoFilterFormat = cbCustomVideoSourceFormat.Text

        If (String.IsNullOrEmpty(cbCustomVideoSourceFrameRate.Text)) Then
            VideoCapture1.Custom_Source.VideoFilterFrameRate = 0.0F
        Else
            VideoCapture1.Custom_Source.VideoFilterFrameRate = Convert.ToDouble(cbCustomVideoSourceFrameRate.Text)
        End If

        If (cbCustomAudioSourceCategory.SelectedIndex = 0) Then
            VideoCapture1.Custom_Source.AudioFilterCategory = VFFilterCategory.AudioCaptureSource
        Else
            VideoCapture1.Custom_Source.AudioFilterCategory = VFFilterCategory.DirectShowFilters
        End If

        VideoCapture1.Custom_Source.AudioFilterName = cbCustomAudioSourceFilter.Text
        VideoCapture1.Custom_Source.AudioFilterFormat = cbCustomAudioSourceFormat.Text
    End Sub

    Private Sub SelectBDASource()

        VideoCapture1.BDA_Source = New BDASourceSettings()
        VideoCapture1.BDA_Source.ReceiverName = cbBDAReceiver.Text
        VideoCapture1.BDA_Source.SourceType = cbBDADeviceStandard.SelectedIndex
        VideoCapture1.BDA_Source.SourceName = cbBDASourceDevice.Text

        If (VideoCapture1.BDA_Source.SourceType = VFBDAType.DVBT) Then

            Dim bdaTuningParameters As VFBDATuningParameters = New VFBDATuningParameters

            bdaTuningParameters.Frequency = Convert.ToInt32(edDVBTFrequency.Text)
            bdaTuningParameters.ONID = Convert.ToInt32(edDVBTONID.Text)
            bdaTuningParameters.SID = Convert.ToInt32(edDVBTSID.Text)
            bdaTuningParameters.TSID = Convert.ToInt32(edDVBTTSID.Text)

            VideoCapture1.BDA_SetParameters(bdaTuningParameters)

        End If
    End Sub

    Private Function SelectIPCameraSource() As IPCameraSourceSettings

        Dim settings As IPCameraSourceSettings = New IPCameraSourceSettings()
        settings.URL = edIPUrl.Text

        Select Case (cbIPCameraType.SelectedIndex)

            Case 0 : settings.Type = VFIPSource.Auto_VLC

            Case 1 : settings.Type = VFIPSource.Auto_FFMPEG

            Case 2 : settings.Type = VFIPSource.Auto_LAV

            Case 3 : settings.Type = VFIPSource.RTSP_Live555

            Case 4 : settings.Type = VFIPSource.HTTP_FFMPEG

            Case 5 : settings.Type = VFIPSource.MMS_WMV

            Case 6 : settings.Type = VFIPSource.RTSP_UDP_FFMPEG

            Case 7 : settings.Type = VFIPSource.RTSP_TCP_FFMPEG

            Case 8 : settings.Type = VFIPSource.RTSP_HTTP_FFMPEG

        End Select

        settings.AudioCapture = cbIPAudioCapture.Checked
        settings.Login = edIPLogin.Text
        settings.Password = edIPPassword.Text
        settings.ForcedFramerate = Convert.ToDouble(edIPForcedFramerate.Text)
        settings.ForcedFramerate_InstanceID = edIPForcedFramerateID.Text(0)
        settings.Debug_Enabled = cbDebugMode.Checked
        settings.Debug_Filename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\VisioForge\\ip_cam_log.txt"
        settings.VLC_ZeroClockJitterEnabled = cbVLCZeroClockJitter.Checked
        settings.VLC_CustomLatency = Convert.ToInt32(edVLCCacheSize.Text)

        If (cbIPCameraONVIF.Checked) Then
            settings.ONVIF_Source = True

            If (cbONVIFProfile.SelectedIndex <> -1) Then
                settings.ONVIF_SourceProfile = cbONVIFProfile.Text
            End If
        End If

        If (cbIPDisconnect.Checked) Then
            settings.DisconnectEventInterval = 5000
        End If

        Return settings

    End Function

    Private Function SelectScreenSource() As ScreenCaptureSourceSettings

        Dim settings As ScreenCaptureSourceSettings = New ScreenCaptureSourceSettings()

        If (rbScreenCaptureWindow.Checked) Then

            settings.Mode = VFScreenCaptureMode.Window

            settings.WindowHandle = IntPtr.Zero

            Try

                settings.WindowHandle = FindWindowByClass(edScreenCaptureWindowName.Text, IntPtr.Zero)

            Catch

            End Try

            If (settings.WindowHandle = IntPtr.Zero) Then

                MessageBox.Show("Incorrect window title for screen capture.")
                Return Nothing

            End If

        Else

            settings.Mode = VFScreenCaptureMode.Screen

        End If

        settings.FrameRate = Convert.ToDouble(edScreenFrameRate.Text)
        settings.FullScreen = rbScreenFullScreen.Checked
        settings.Top = Convert.ToInt32(edScreenTop.Text)
        settings.Bottom = Convert.ToInt32(edScreenBottom.Text)
        settings.Left = Convert.ToInt32(edScreenLeft.Text)
        settings.Right = Convert.ToInt32(edScreenRight.Text)
        settings.GrabMouseCursor = cbScreenCapture_GrabMouseCursor.Checked
        settings.DisplayIndex = Convert.ToInt32(cbScreenCaptureDisplayIndex.Text)
        settings.AllowDesktopDuplicationEngine = cbScreenCapture_DesktopDuplication.Checked

        Return settings
    End Function

    Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btStop.Click

        'clear VU Meters
        Dim data(19) As Int32

        peakMeterCtrl1.SetData(data, 0, 19)
        peakMeterCtrl1.Stop()

        VideoCapture1.Stop()

        If cbUseAdditionalScreens.Checked Then
            pnScreen1.Refresh()
            pnScreen2.Refresh()
            pnScreen3.Refresh()
        End If

        waveformPainter1.Clear()
        waveformPainter2.Clear()

        volumeMeter1.Clear()
        volumeMeter2.Clear()

        cbPIPDevices.Items.Clear()

        lbFilters.Items.Clear()
        VideoCapture1.Video_Filters_Clear()

    End Sub

    Private Sub btVideoSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btVideoSettings.Click

        Dim sName As String = cbVideoCodecs.Text

        If VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.Video_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else

            If VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.Video_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub cbAudioCodecs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudioCodecs.SelectedIndexChanged

        Dim sName As String = cbAudioCodecs.Text
        btAudioSettings.Enabled = VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig)

    End Sub

    Private Sub cbAudioInputSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs)

        VideoCapture1.Audio_CaptureDevice_Format = cbAudioInputFormat.Text

    End Sub

    Private Sub cbAudioInputLine_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs)

        VideoCapture1.Audio_CaptureDevice_Line = cbAudioInputLine.Text

    End Sub

    Private Sub cbAudioOutputDevice_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudioOutputDevice.SelectedIndexChanged

        VideoCapture1.Audio_OutputDevice = cbAudioOutputDevice.Text

    End Sub

    Private Sub cbCrossbarInput_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCrossbarInput.SelectedIndexChanged

        Dim RelatedInput As String = "", RelatedOutput As String = ""

        If cbCrossbarInput.SelectedIndex <> -1 Then
            VideoCapture1.Video_CaptureDevice_CrossBar_GetRelated(cbCrossbarInput.Text, cbCrossbarOutput.Text, RelatedInput, RelatedOutput)
        End If

    End Sub

    Private Sub cbCrossbarOutput_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCrossbarOutput.SelectedIndexChanged

        cbCrossbarInput.Items.Clear()

        If cbCrossbarOutput.SelectedIndex <> -1 Then

            Dim inputs As List(Of String)

            inputs = VideoCapture1.Video_CaptureDevice_CrossBar_GetInputsForOutput(cbCrossbarOutput.Text)
            For i As Integer = 0 To inputs.Count - 1
                cbCrossbarInput.Items.Add(inputs.Item(i))
            Next i
        End If

        Dim input1 As String
        input1 = VideoCapture1.Video_CaptureDevice_CrossBar_GetConnectedInputForOutput(cbCrossbarOutput.Text)

        If input1 <> "" Then
            cbCrossbarInput.SelectedIndex = cbCrossbarInput.Items.IndexOf(input1)
        Else
            cbCrossbarInput.SelectedIndex = -1
        End If

        cbCrossbarInput_SelectedIndexChanged(sender, e)

    End Sub

    Private Sub cbTVChannel_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTVChannel.SelectedIndexChanged
        If cbTVChannel.SelectedIndex <> -1 Then

            Dim k As Integer = Convert.ToInt32(cbTVChannel.Text)

            If k <= 10000 Then
                'channel
                VideoCapture1.TVTuner_Channel = k
            Else

                VideoCapture1.TVTuner_Channel = -1
                'must be -1 to use frequency

                VideoCapture1.TVTuner_Frequency = k
            End If

            VideoCapture1.TVTuner_Apply()
            VideoCapture1.TVTuner_Read()
            edVideoFreq.Text = VideoCapture1.TVTuner_VideoFrequency.ToString()
            edAudioFreq.Text = VideoCapture1.TVTuner_AudioFrequency.ToString()
        End If

    End Sub

    Private Sub cbTVTuner_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTVTuner.SelectedIndexChanged

        If cbTVTuner.Items.Count > 0 And cbTVTuner.SelectedIndex <> -1 Then
            VideoCapture1.TVTuner_Name = cbTVTuner.Text
            VideoCapture1.TVTuner_Read()

            cbTVMode.Items.Clear()
            For i As Integer = 0 To VideoCapture1.TVTuner_Modes.Count - 1
                cbTVMode.Items.Add(VideoCapture1.TVTuner_Modes.Item(i))
            Next i

            edVideoFreq.Text = Convert.ToString(VideoCapture1.TVTuner_VideoFrequency)
            edAudioFreq.Text = Convert.ToString(VideoCapture1.TVTuner_AudioFrequency)
            cbTVInput.SelectedIndex = cbTVInput.Items.IndexOf(VideoCapture1.TVTuner_InputType)
            cbTVMode.SelectedIndex = cbTVMode.Items.IndexOf(VideoCapture1.TVTuner_Mode)
            cbTVSystem.SelectedIndex = cbTVSystem.Items.IndexOf(VideoCapture1.TVTuner_TVFormat)
            cbTVCountry.SelectedIndex = cbTVCountry.Items.IndexOf(VideoCapture1.TVTuner_Country)
        End If

    End Sub

    Private Sub cbVideoCodecs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbVideoCodecs.SelectedIndexChanged

        Dim sName As String = cbVideoCodecs.Text
        btVideoSettings.Enabled = VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig)

    End Sub

    Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed

        If VideoCapture1.Status = VFVideoCaptureStatus.Work Then
            VideoCapture1.Stop()
        End If

    End Sub

    Private Sub cbGreyscale_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbGreyscale.CheckedChanged

        Dim intf As IVFVideoEffectGrayscale
        Dim effect = VideoCapture1.Video_Effects_Get("Grayscale")
        If (IsNothing(effect)) Then
            intf = New VFVideoEffectGrayscale(cbGreyscale.Checked)
            VideoCapture1.Video_Effects_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGreyscale.Checked
            End If
        End If

    End Sub

    Private Sub btSaveScreenshot_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSaveScreenshot.Click

        Dim s As String

        Dim dt As DateTime = DateTime.Now

        s = dt.Hour.ToString() + "_" + dt.Minute.ToString() + "_" + dt.Second.ToString() + "_" + dt.Millisecond.ToString()

        Select Case (cbImageType.SelectedIndex)
            Case 0 : VideoCapture1.Frame_Save(edScreenshotsFolder.Text + "\" + s + ".bmp", VFImageFormat.BMP, 0)
            Case 1 : VideoCapture1.Frame_Save(edScreenshotsFolder.Text + "\" + s + ".jpg", VFImageFormat.JPEG, tbJPEGQuality.Value)
            Case 2 : VideoCapture1.Frame_Save(edScreenshotsFolder.Text + "\" + s + ".gif", VFImageFormat.GIF, 0)
            Case 3 : VideoCapture1.Frame_Save(edScreenshotsFolder.Text + "\" + s + ".png", VFImageFormat.PNG, 0)
            Case 4 : VideoCapture1.Frame_Save(edScreenshotsFolder.Text + "\" + s + ".tiff", VFImageFormat.TIFF, 0)
        End Select

    End Sub

    Private Sub tbJPEGQuality_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbJPEGQuality.Scroll

        lbJPEGQuality.Text = tbJPEGQuality.Value.ToString()

    End Sub

    Private Sub btFont_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btFont.Click

        If fontDialog1.ShowDialog() = DialogResult.OK Then
            edTextLogoSampleText.Font = fontDialog1.Font
            btTextLogoUpdateParams_Click(sender, e)
        End If

    End Sub

    Private Sub cbTextLogo_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTextLogo.CheckedChanged

        btTextLogoUpdateParams_Click(sender, e)

    End Sub

    Private Sub cbInvert_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbInvert.CheckedChanged

        Dim intf As IVFVideoEffectInvert
        Dim effect = VideoCapture1.Video_Effects_Get("Invert")
        If (IsNothing(effect)) Then
            intf = New VFVideoEffectInvert(cbInvert.Checked)
            VideoCapture1.Video_Effects_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbInvert.Checked
            End If
        End If

    End Sub

    Private Sub tbAdjBrightness_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAdjBrightness.Scroll

        VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(VFVideoHardwareAdjustment.Brightness, tbAdjBrightness.Value, cbAdjBrightnessAuto.Checked)
        lbAdjBrightnessCurrent.Text = "Current: " + Convert.ToString(tbAdjBrightness.Value)

    End Sub

    Private Sub tbAdjContrast_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAdjContrast.Scroll

        VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(VFVideoHardwareAdjustment.Contrast, tbAdjContrast.Value, cbAdjContrastAuto.Checked)
        lbAdjContrastCurrent.Text = "Current: " + Convert.ToString(tbAdjContrast.Value)

    End Sub

    Private Sub tbAdjHue_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAdjHue.Scroll

        VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(VFVideoHardwareAdjustment.Hue, tbAdjHue.Value, cbAdjHueAuto.Checked)
        lbAdjHueCurrent.Text = "Current: " + Convert.ToString(tbAdjHue.Value)

    End Sub

    Private Sub tbAdjSaturation_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAdjSaturation.Scroll

        VideoCapture1.Video_CaptureDevice_VideoAdjust_SetValue(VFVideoHardwareAdjustment.Saturation, tbAdjSaturation.Value, cbAdjSaturationAuto.Checked)
        lbAdjSaturationCurrent.Text = "Current: " + Convert.ToString(tbAdjSaturation.Value)

    End Sub

    Private Sub cbUseAudioInputFromVideoCaptureDevice_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbUseAudioInputFromVideoCaptureDevice.CheckedChanged

        If Not (cbVideoInputDevice.Text = "" Or IsDBNull(cbVideoInputDevice.Text)) Then
            VideoCapture1.Video_CaptureDevice_IsAudioSource = cbUseAudioInputFromVideoCaptureDevice.Checked
            cbAudioInputDevice_SelectedIndexChanged(sender, e)

            cbAudioInputDevice.Enabled = Not cbUseAudioInputFromVideoCaptureDevice.Checked
            btAudioInputDeviceSettings.Enabled = Not cbUseAudioInputFromVideoCaptureDevice.Checked
        End If

    End Sub

    Private Sub cbCustomVideoCodecs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCustomVideoCodecs.SelectedIndexChanged

        Dim sName As String = cbCustomVideoCodecs.Text
        btCustomVideoCodecSettings.Enabled = (VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub cbCustomAudioCodecs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCustomAudioCodecs.SelectedIndexChanged

        Dim sName As String = cbCustomAudioCodecs.Text
        btCustomAudioCodecSettings.Enabled = (VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub cbAudioCodecs2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudioCodecs2.SelectedIndexChanged

        Dim sName As String = cbAudioCodecs2.Text
        btAudioSettings2.Enabled = (VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub cbCustomDSFilterV_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCustomDSFilterV.SelectedIndexChanged

        Dim sName As String = cbCustomDSFilterV.Text
        btCustomDSFiltersVSettings.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub cbCustomDSFilterA_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCustomDSFilterA.SelectedIndexChanged

        Dim sName As String = cbCustomDSFilterA.Text
        btCustomDSFiltersASettings.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub cbCustomMuxer_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCustomMuxer.SelectedIndexChanged

        Dim sName As String = cbCustomMuxer.Text
        btCustomMuxerSettings.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub cbCustomFilewriter_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbCustomFilewriter.SelectedIndexChanged

        Dim sName As String = cbCustomFilewriter.Text
        btCustomFilewriterSettings.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Or VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

    End Sub

    Private Sub rbVR_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles rbVR.CheckedChanged, rbVMR9.CheckedChanged, rbNone.CheckedChanged, rbEVR.CheckedChanged, rbDirect2D.CheckedChanged

        cbScreenFlipVertical.Enabled = rbVMR9.Checked Or rbDirect2D.Checked
        cbScreenFlipHorizontal.Enabled = rbVMR9.Checked Or rbDirect2D.Checked

        If Tag = 1 Then
            If rbVMR9.Checked Then
                VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.VMR9
            ElseIf rbEVR.Checked Then
                VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.EVR
            ElseIf rbVR.Checked Then
                VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.VideoRenderer
            ElseIf (rbDirect2D.Checked) Then
                VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.Direct2D
            Else
                VideoCapture1.Video_Renderer.Video_Renderer = VFVideoRenderer.None
            End If

        End If

    End Sub

    Private Sub btVideoCaptureDeviceSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btVideoCaptureDeviceSettings.Click

        VideoCapture1.Video_CaptureDevice_SettingsDialog_Show(IntPtr.Zero, cbVideoInputDevice.Text)

    End Sub

    Private Sub btAudioInputDeviceSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btAudioInputDeviceSettings.Click

        VideoCapture1.Audio_CaptureDevice_SettingsDialog_Show(IntPtr.Zero, cbVideoInputDevice.Text)

    End Sub

    Private Sub btConnect_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btConnect.Click

        Dim input1 As String

        If (cbCrossbarInput.SelectedIndex <> -1) And (cbCrossbarOutput.SelectedIndex <> -1) Then
            VideoCapture1.Video_CaptureDevice_CrossBar_Connect(cbCrossbarInput.Text, cbCrossbarOutput.Text, cbConnectRelated.Checked)

            lbRotes.Items.Clear()
            For i As Integer = 0 To cbCrossbarOutput.Items.Count - 1

                input1 = VideoCapture1.Video_CaptureDevice_CrossBar_GetConnectedInputForOutput(cbCrossbarOutput.Items.Item(i).ToString())
                lbRotes.Items.Add("Input: " + input1 + ", Output: " + cbCrossbarOutput.Items.Item(i))

            Next i
        End If

    End Sub

    Private Sub btCustomAudioCodecSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btCustomAudioCodecSettings.Click

        Dim sName As String = cbCustomAudioCodecs.Text

        If VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.Audio_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.Audio_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btCustomDSFiltersASettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btCustomDSFiltersASettings.Click

        Dim sName As String = cbCustomDSFilterA.Text

        If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btCustomDSFiltersVSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btCustomDSFiltersVSettings.Click

        Dim sName As String = cbCustomDSFilterV.Text

        If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btCustomFilewriterSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btCustomFilewriterSettings.Click

        Dim sName As String = cbCustomFilewriter.Text

        If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btCustomMuxerSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btCustomMuxerSettings.Click

        Dim sName As String = cbCustomMuxer.Text

        If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btCustomVideoCodecSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btCustomVideoCodecSettings.Click

        Dim sName As String = cbCustomVideoCodecs.Text

        If VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.Video_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.Video_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.Video_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btDVFF_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVFF.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.FastestFwd)

    End Sub

    Private Sub btDVPause_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVPause.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.Pause)

    End Sub

    Private Sub btDVRewind_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVRewind.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.Rew)

    End Sub

    Private Sub btDVPlay_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVPlay.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.Play)

    End Sub

    Private Sub btDVStepFWD_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVStepFWD.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.StepFw)

    End Sub

    Private Sub btDVStepRev_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVStepRev.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.StepRev)

    End Sub

    Private Sub btDVStop_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btDVStop.Click

        VideoCapture1.DV_SendCommand(VFDVCommand.Stop)

    End Sub

    Private Sub btRefreshClients_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btRefreshClients.Click

        Dim dns1 As String = "", address As String = ""

        Dim port As Integer = 0

        lbNetworkClients.Items.Clear()

        For i As Integer = 0 To VideoCapture1.Network_Streaming_WMV_Clients_GetCount - 1

            VideoCapture1.Network_Streaming_WMV_Clients_GetInfo(i, port, address, dns1)

            Dim client As String = "ID: " + i + ", Port: " + port + ", Address: " + address + ", DNS: " + dns1
            lbNetworkClients.Items.Add(client)

        Next i

    End Sub

    Private Sub btSelectImage_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSelectImage.Click

        If openFileDialog2.ShowDialog() = DialogResult.OK Then
            edImageLogoFilename.Text = openFileDialog2.FileName
        End If

    End Sub

    Private Sub btStartTune_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btStartTune.Click

        Const KHz As Integer = 1000
        Const MHz As Integer = 1000000

        VideoCapture1.TVTuner_Read()
        cbTVChannel.Items.Clear()

        If (cbTVMode.SelectedIndex <> -1) And (cbTVMode.Text = "FM Radio") Then
            VideoCapture1.TVTuner_FM_Tuning_StartFrequency = 100 * MHz
            VideoCapture1.TVTuner_FM_Tuning_StopFrequency = 110 * MHz
            VideoCapture1.TVTuner_FM_Tuning_Step = 100 * KHz
        End If

        VideoCapture1.TVTuner_TuneChannels_Start()

    End Sub

    Private Sub btStopTune_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btStopTune.Click

        VideoCapture1.TVTuner_TuneChannels_Stop()

    End Sub

    Private Sub btUseThisChannel_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btUseThisChannel.Click
        If Convert.ToInt32(edChannel.Text) <= 10000 Then
            'channel
            VideoCapture1.TVTuner_Channel = Convert.ToInt32(edChannel.Text)
        Else

            VideoCapture1.TVTuner_Channel = -1
            'must be -1 to use frequency

            VideoCapture1.TVTuner_Frequency = Convert.ToInt32(edChannel.Text)

        End If

        VideoCapture1.TVTuner_Apply()
        VideoCapture1.TVTuner_Read()
        edVideoFreq.Text = VideoCapture1.TVTuner_VideoFrequency.ToString()
        edAudioFreq.Text = VideoCapture1.TVTuner_AudioFrequency.ToString()

    End Sub

    Private Sub cbTVCountry_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTVCountry.SelectedIndexChanged
        If cbTVCountry.SelectedIndex <> -1 Then

            VideoCapture1.TVTuner_Country = cbTVCountry.Text
            edTVDefaultFormat.Text = VideoCapture1.TVTuner_Countries_GetPreferredFormatForCountry(VideoCapture1.TVTuner_Country)

            If VideoCapture1.Status = VFVideoCaptureStatus.Work Then
                VideoCapture1.TVTuner_Apply()
                VideoCapture1.TVTuner_Read()
            End If

        End If
    End Sub

    Private Sub cbImageLogo_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbImageLogo.CheckedChanged

        Dim imageLogo As IVFVideoEffectImageLogo
        Dim effect = VideoCapture1.Video_Effects_Get("ImageLogo")
        If (IsNothing(effect)) Then
            imageLogo = New VFVideoEffectImageLogo(cbImageLogo.Checked)
            VideoCapture1.Video_Effects_Add(imageLogo)
        Else
            imageLogo = effect
        End If

        If (IsNothing(imageLogo)) Then
            MessageBox.Show("Unable to configure image logo effect.")
            Return
        End If

        imageLogo.Enabled = cbImageLogo.Checked
        imageLogo.Filename = edImageLogoFilename.Text
        imageLogo.Left = Convert.ToUInt32(edImageLogoLeft.Text)
        imageLogo.Top = Convert.ToUInt32(edImageLogoTop.Text)
        imageLogo.TransparencyLevel = tbImageLogoTransp.Value
        imageLogo.ColorKey = pnImageLogoColorKey.ForeColor
        imageLogo.UseColorKey = cbImageLogoUseColorKey.Checked
        imageLogo.AnimationEnabled = True

        If (cbImageLogoShowAlways.Checked) Then
            imageLogo.StartTime = 0
            imageLogo.StopTime = 0
        Else
            imageLogo.StartTime = Convert.ToInt32(edImageLogoStartTime.Text)
            imageLogo.StopTime = Convert.ToInt32(edImageLogoStopTime.Text)
        End If

    End Sub

    Private Sub cbImageLogoShowAlways_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbImageLogoShowAlways.CheckedChanged

        If Tag = 1 Then
            edImageLogoStartTime.Enabled = Not cbImageLogoShowAlways.Checked
            edImageLogoStopTime.Enabled = Not cbImageLogoShowAlways.Checked
            lbGraphicLogoStartTime.Enabled = Not cbImageLogoShowAlways.Checked
            lbGraphicLogoStopTime.Enabled = Not cbImageLogoShowAlways.Checked

            cbImageLogo_CheckedChanged(sender, e)
        End If

    End Sub

    Private Sub cbStretch_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbStretch.CheckedChanged

        VideoCapture1.Video_Renderer.StretchMode = cbStretch.Checked
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub cbTVSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTVMode.SelectedIndexChanged

        If cbTVMode.SelectedIndex <> -1 Then
            VideoCapture1.TVTuner_Mode = cbTVMode.Text
            VideoCapture1.TVTuner_Apply()
            VideoCapture1.TVTuner_Read()
            cbTVChannel.Items.Clear()
            edVideoFreq.Text = VideoCapture1.TVTuner_VideoFrequency.ToString()
            edAudioFreq.Text = VideoCapture1.TVTuner_AudioFrequency.ToString()
        End If

    End Sub

    Private Sub cbTVInput_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTVInput.SelectedIndexChanged
        If cbTVInput.SelectedIndex <> -1 Then
            VideoCapture1.TVTuner_InputType = cbTVInput.Text
            VideoCapture1.TVTuner_Apply()
            VideoCapture1.TVTuner_Read()
            edVideoFreq.Text = VideoCapture1.TVTuner_VideoFrequency.ToString()
            edAudioFreq.Text = VideoCapture1.TVTuner_AudioFrequency.ToString()
        End If

    End Sub

    Private Sub cbTVSystem_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbTVSystem.SelectedIndexChanged

        If cbTVSystem.SelectedIndex <> -1 Then
            VideoCapture1.TVTuner_TVFormat = VideoCapture1.TVTuner_TVFormat_FromString(cbTVSystem.Text)
            VideoCapture1.TVTuner_Apply()
            VideoCapture1.TVTuner_Read()
            edVideoFreq.Text = VideoCapture1.TVTuner_VideoFrequency.ToString()
            edAudioFreq.Text = VideoCapture1.TVTuner_AudioFrequency.ToString()
        End If

    End Sub

    Private Sub cbUseBestAudioInputCheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbUseBestAudioInputFormat.CheckedChanged

        cbAudioInputFormat.Enabled = Not cbUseBestAudioInputFormat.Checked

    End Sub

    Private Sub cbUseBestVideoInputCheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbUseBestVideoInputFormat.CheckedChanged

        cbVideoInputFormat.Enabled = Not cbUseBestVideoInputFormat.Checked

    End Sub

    Private Sub cbUseSpecialFilewriter_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbUseSpecialFilewriter.CheckedChanged

        cbCustomFilewriter.Enabled = cbUseSpecialFilewriter.Checked
        btCustomFilewriterSettings.Enabled = cbUseSpecialFilewriter.Checked

    End Sub

    Private Sub tbAudioVolume_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudioVolume.Scroll

        VideoCapture1.Audio_OutputDevice_Volume_Set(tbAudioVolume.Value)

    End Sub

    Private Sub tbAudioBalance_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudioBalance.Scroll

        VideoCapture1.Audio_OutputDevice_Balance_Set(tbAudioBalance.Value)
        VideoCapture1.Audio_OutputDevice_Balance_Get()

    End Sub

    Private Sub btAudioSettings2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btAudioSettings2.Click

        Dim sName As String = cbAudioCodecs2.Text

        If VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.Default) Then
            VideoCapture.Audio_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        Else
            If VideoCapture.Audio_Codec_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.Audio_Codec_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btSelectScreenshotsFolder_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSelectScreenshotsFolder.Click

        If folderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            edScreenshotsFolder.Text = folderBrowserDialog1.SelectedPath
        End If

    End Sub

    Private Sub btSelectWMVProfileNetwork_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSelectWMVProfileNetwork.Click

        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            edNetworkStreamingWMVProfile.Text = openFileDialog1.FileName
        End If

    End Sub

    Private Sub btPause_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btPause.Click

        VideoCapture1.Pause()

    End Sub

    Private Sub btResume_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btResume.Click

        VideoCapture1.Resume()

    End Sub

    Private Sub btMPEGEncoderShowDialog_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btMPEGEncoderShowDialog.Click

        If cbMPEGEncoder.SelectedIndex <> -1 Then
            VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name = cbMPEGEncoder.Text
            VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_ShowDialog(IntPtr.Zero)
        End If

    End Sub

    Private Sub cbUncVideo_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbUncVideo.CheckedChanged

        cbVideoCodecs.Enabled = Not cbUncVideo.Checked
        btVideoSettings.Enabled = Not cbUncVideo.Checked
        cbDecodeToRGB.Enabled = cbUncVideo.Checked

    End Sub

    Private Sub cbFilters_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFilters.SelectedIndexChanged

        If cbFilters.SelectedIndex <> -1 Then

            Dim sName As String = cbFilters.Text
            btFilterSettings.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default)) Or (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

        End If

    End Sub

    Private Sub btFilterSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btFilterSettings.Click

        Dim sName As String = cbFilters.Text

        If (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default)) Then
            VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
        ElseIf (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig)) Then
            VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)

        End If

    End Sub

    Private Sub lbFilters_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles lbFilters.SelectedIndexChanged

        If lbFilters.SelectedIndex <> -1 Then

            Dim sName As String = lbFilters.Text
            btFilterSettings2.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default)) Or (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig))

        End If

    End Sub

    Private Sub btFilterSettings2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btFilterSettings2.Click

        If lbFilters.SelectedIndex <> -1 Then

            Dim sName As String = lbFilters.Text

            If (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default)) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
            ElseIf (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig)) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)

            End If

        End If

    End Sub

    Private Sub btFilterDeleteAll_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btFilterDeleteAll.Click

        lbFilters.Items.Clear()
        VideoCapture1.Video_Filters_Clear()

    End Sub

    Private Sub tbGraphicLogoTransp_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbImageLogoTransp.Scroll

        cbImageLogo_CheckedChanged(sender, e)

    End Sub

    Private Sub cbGraphicLogoUseColorKey_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbImageLogoUseColorKey.CheckedChanged

        cbImageLogo_CheckedChanged(sender, e)

    End Sub

    Private Sub pnGraphicLogoColorKey_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnImageLogoColorKey.Click

        colorDialog1.Color = pnImageLogoColorKey.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnImageLogoColorKey.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub btOSDInit_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDInit.Click

        VideoCapture1.OSD_Init()

    End Sub

    Private Sub btOSDDeinit_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDDeinit.Click

        VideoCapture1.OSD_Destroy()

    End Sub

    Private Sub btOSDClearLayers_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDClearLayers.Click

        VideoCapture1.OSD_Layers_Clear()
        lbOSDLayers.Items.Clear()

    End Sub

    Private Sub btOSDLayerAdd_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDLayerAdd.Click

        VideoCapture1.OSD_Layers_Create(Convert.ToInt32(edOSDLayerLeft.Text), Convert.ToInt32(edOSDLayerTop.Text), Convert.ToInt32(edOSDLayerWidth.Text), Convert.ToInt32(edOSDLayerHeight.Text))
        lbOSDLayers.Items.Add("layer " + Convert.ToString(lbOSDLayers.Items.Count + 1))

    End Sub

    Private Sub btOSDApplyLayer_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDApplyLayer.Click

        If lbOSDLayers.SelectedIndex <> -1 Then
            VideoCapture1.OSD_Layers_Apply(lbOSDLayers.SelectedIndex)
        End If

    End Sub

    Private Sub btOSDSelectImage_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDSelectImage.Click

        If openFileDialog2.ShowDialog() = DialogResult.OK Then
            edOSDImageFilename.Text = openFileDialog2.FileName
        End If

    End Sub

    Private Sub btOSDImageDraw_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDImageDraw.Click

        If (lbOSDLayers.SelectedIndex <> -1) Then

            If (cbOSDImageTranspColor.Checked) Then
                VideoCapture1.OSD_Layers_Draw_ImageFromFile(lbOSDLayers.SelectedIndex, edOSDImageFilename.Text, Convert.ToInt32(edOSDImageLeft.Text), Convert.ToInt32(edOSDImageTop.Text), True, pnOSDColorKey.BackColor)
            Else
                VideoCapture1.OSD_Layers_Draw_ImageFromFile(lbOSDLayers.SelectedIndex, edOSDImageFilename.Text, Convert.ToInt32(edOSDImageLeft.Text), Convert.ToInt32(edOSDImageTop.Text), False, Color.Black)
            End If

        End If

    End Sub

    Private Sub btOSDSelectFont_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDSelectFont.Click

        If fontDialog1.ShowDialog() = DialogResult.OK Then
            edOSDText.Font = fontDialog1.Font
            edOSDText.ForeColor = fontDialog1.Color
        End If

    End Sub

    Private Sub btOSDTextDraw_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDTextDraw.Click

        If lbOSDLayers.SelectedIndex <> -1 Then
            Dim fnt As Font
            Dim color1 As Color

            fnt = edOSDText.Font
            color1 = edOSDText.ForeColor

            VideoCapture1.OSD_Layers_Draw_Text(lbOSDLayers.SelectedIndex, Convert.ToInt32(edOSDTextLeft.Text), Convert.ToInt32(edOSDTextTop.Text), edOSDText.Text, fnt, color1)
        End If

    End Sub

    Private Sub btOSDSetTransp_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btOSDSetTransp.Click

        If lbOSDLayers.SelectedIndex <> -1 Then
            VideoCapture1.OSD_Layers_SetTransparency(lbOSDLayers.SelectedIndex, tbOSDTranspLevel.Value)
            VideoCapture1.OSD_Layers_Apply(lbOSDLayers.SelectedIndex)
        End If

    End Sub

    Private Sub cbMPEGVideoDecoder_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbMPEGVideoDecoder.SelectedIndexChanged

        Dim sName As String

        If cbMPEGVideoDecoder.SelectedIndex < 1 Then
            btMPEGVidDecSetting.Enabled = False
        Else
            sName = cbMPEGVideoDecoder.Text
            btMPEGVidDecSetting.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Or (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig)))
        End If

    End Sub

    Private Sub cbMPEGAudioDecoder_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbMPEGAudioDecoder.SelectedIndexChanged

        Dim sName As String

        If cbMPEGAudioDecoder.SelectedIndex < 1 Then
            btMPEGAudDecSettings.Enabled = False
        Else
            sName = cbMPEGVideoDecoder.Text
            btMPEGAudDecSettings.Enabled = (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Or (VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig)))
        End If

    End Sub

    Private Sub btMPEGVidDecSetting_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btMPEGVidDecSetting.Click

        Dim sName As String

        If cbMPEGVideoDecoder.SelectedIndex > 0 Then
            sName = cbMPEGVideoDecoder.Text

            If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
            ElseIf VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btMPEGAudDecSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btMPEGAudDecSettings.Click

        Dim sName As String

        If cbMPEGAudioDecoder.SelectedIndex > 0 Then
            sName = cbMPEGAudioDecoder.Text

            If VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.Default) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.Default)
            ElseIf VideoCapture.DirectShow_Filter_Has_Dialog(sName, VFPropertyPage.VFWCompConfig) Then
                VideoCapture.DirectShow_Filter_Show_Dialog(IntPtr.Zero, sName, VFPropertyPage.VFWCompConfig)
            End If
        End If

    End Sub

    Private Sub btScreenCaptureUpdate_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btScreenCaptureUpdate.Click

        VideoCapture1.Screen_Capture_UpdateParameters(Convert.ToInt32(edScreenLeft.Text), Convert.ToInt32(edScreenTop.Text), cbScreenCapture_GrabMouseCursor.Checked)

    End Sub

    Private Sub cbPIPDevice_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbPIPDevice.SelectedIndexChanged

        If cbPIPDevice.SelectedIndex <> -1 Then

            VideoCapture1.Video_CaptureDevice = cbPIPDevice.Text

            cbPIPFormat.Items.Clear()

            Dim deviceItem = (From info In VideoCapture1.Video_CaptureDevicesInfo Where info.Name = cbPIPDevice.Text)?.First()
            If Not IsNothing(deviceItem) Then
                Dim formats = deviceItem.VideoFormats
                For Each item As String In formats
                    cbPIPFormat.Items.Add(item)
                Next

                If cbPIPFormat.Items.Count > 0 Then
                    cbPIPFormat.SelectedIndex = 0
                End If

                cbPIPFrameRate.Items.Clear()

                Dim frameRate = deviceItem.VideoFrameRates
                For Each item As String In frameRate
                    cbPIPFrameRate.Items.Add(item)
                Next

                If cbPIPFrameRate.Items.Count > 0 Then
                    cbPIPFrameRate.SelectedIndex = 0
                End If

                cbPIPInput.Items.Clear()

                VideoCapture1.PIP_Sources_Device_GetCrossbar(cbPIPDevice.Text)
                For i As Integer = 0 To VideoCapture1.PIP_Sources_Device_GetCrossbarInputs().Count - 1
                    cbPIPInput.Items.Add(VideoCapture1.PIP_Sources_Device_GetCrossbarInputs().Item(i))
                Next i

                If (cbPIPInput.Items.Count > 0) Then
                    cbPIPInput.SelectedIndex = 0
                End If
            End If
        End If

    End Sub

    Private Sub cbPIPFormatUseBest_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbPIPFormatUseBest.CheckedChanged

        cbPIPFormat.Enabled = Not cbPIPFormatUseBest.Checked

    End Sub

    Private Sub btPIPAddDevice_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btPIPAddDevice.Click

        Dim format As String
        Dim frame_rate As String
        Dim input As String

        If cbPIPDevice.SelectedIndex <> -1 Then
            If cbPIPFrameRate.SelectedIndex <> -1 Then
                frame_rate = cbPIPFrameRate.Text
            Else
                frame_rate = "0"
            End If

            If cbPIPFormat.SelectedIndex <> -1 Then
                format = cbPIPFormat.Text
            Else
                format = ""
            End If

            If (cbPIPInput.SelectedIndex <> -1) Then
                input = cbPIPInput.Text
            Else
                input = ""
            End If

            VideoCapture1.PIP_Sources_Add_VideoCaptureDevice(
                cbPIPDevice.Text,
                format,
                cbPIPFormatUseBest.Checked,
                Convert.ToDouble(frame_rate),
                input,
                Convert.ToInt32(edPIPVidCapLeft.Text),
                Convert.ToInt32(edPIPVidCapTop.Text),
                Convert.ToInt32(edPIPVidCapWidth.Text),
                Convert.ToInt32(edPIPVidCapHeight.Text))

            cbPIPDevices.Items.Add(cbPIPDevice.Text)
        End If

    End Sub

    Private Sub btTextLogoUpdateParams_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btTextLogoUpdateParams.Click

        Dim rotate As VFTextRotationMode
        Dim flip As VFTextFlipMode

        Dim stringFormat = New StringFormat()

        If (cbTextLogoVertical.Checked) Then
            stringFormat.FormatFlags = stringFormat.FormatFlags Xor StringFormatFlags.DirectionVertical
        End If

        If (cbTextLogoRightToLeft.Checked) Then
            stringFormat.FormatFlags = stringFormat.FormatFlags Xor StringFormatFlags.DirectionRightToLeft
        End If

        stringFormat.Alignment = cbTextLogoAlign.SelectedIndex

        Dim textLogo As IVFVideoEffectTextLogo
        Dim effect = VideoCapture1.Video_Effects_Get("TextLogo")
        If (IsNothing(effect)) Then
            textLogo = New VFVideoEffectTextLogo(cbTextLogo.Checked)
            VideoCapture1.Video_Effects_Add(textLogo)
        Else
            textLogo = effect
        End If

        If (IsNothing(textLogo)) Then
            MessageBox.Show("Unable to configure text logo effect.")
            Return
        End If

        textLogo.Enabled = cbTextLogo.Checked
        textLogo.Text = edTextLogo.Text
        textLogo.Left = Convert.ToInt32(edTextLogoLeft.Text)
        textLogo.Top = Convert.ToInt32(edTextLogoTop.Text)
        textLogo.Font = fontDialog1.Font
        textLogo.FontColor = fontDialog1.Color

        textLogo.BackgroundTransparent = cbTextLogoTranspBG.Checked
        textLogo.BackgroundColor = pnTextLogoBGColor.BackColor
        textLogo.StringFormat = stringFormat
        textLogo.Antialiasing = cbTextLogoAntialiasing.SelectedIndex
        textLogo.DrawQuality = cbTextLogoDrawMode.SelectedIndex

        If (cbTextLogoUseRect.Checked) Then
            textLogo.RectWidth = Convert.ToInt32(edTextLogoWidth.Text)
            textLogo.RectHeight = Convert.ToInt32(edTextLogoHeight.Text)
        Else
            textLogo.RectWidth = 0
            textLogo.RectHeight = 0
        End If

        If (rbTextLogoDegree0.Checked) Then
            rotate = VFTextRotationMode.RmNone
        ElseIf (rbTextLogoDegree90.Checked) Then
            rotate = VFTextRotationMode.Rm90
        ElseIf (rbTextLogoDegree180.Checked) Then
            rotate = VFTextRotationMode.Rm180
        Else
            rotate = VFTextRotationMode.Rm270
        End If

        If (rbTextLogoFlipNone.Checked) Then
            flip = VFTextFlipMode.None
        ElseIf (rbTextLogoFlipX.Checked) Then
            flip = VFTextFlipMode.X
        ElseIf (rbTextLogoFlipY.Checked) Then
            flip = VFTextFlipMode.Y
        Else
            flip = VFTextFlipMode.XAndY
        End If

        textLogo.RotationMode = rotate
        textLogo.FlipMode = flip

        textLogo.GradientEnabled = cbTextLogoGradientEnabled.Checked
        textLogo.GradientMode = cbTextLogoGradMode.SelectedIndex
        textLogo.GradientColor1 = pnTextLogoGradColor1.BackColor
        textLogo.GradientColor2 = pnTextLogoGradColor2.BackColor

        textLogo.BorderMode = cbTextLogoEffectrMode.SelectedIndex
        textLogo.BorderInnerColor = pnTextLogoInnerColor.BackColor
        textLogo.BorderOuterColor = pnTextLogoOuterColor.BackColor
        textLogo.BorderInnerSize = Convert.ToInt32(edTextLogoInnerSize.Text)
        textLogo.BorderOuterSize = Convert.ToInt32(edTextLogoOuterSize.Text)

        textLogo.Shape = cbTextLogoShapeEnabled.Checked
        textLogo.ShapeLeft = Convert.ToInt32(edTextLogoShapeLeft.Text)
        textLogo.ShapeTop = Convert.ToInt32(edTextLogoShapeTop.Text)
        textLogo.ShapeType = cbTextLogoShapeType.SelectedIndex
        textLogo.ShapeWidth = Convert.ToInt32(edTextLogoShapeWidth.Text)
        textLogo.ShapeHeight = Convert.ToInt32(edTextLogoShapeHeight.Text)
        textLogo.ShapeColor = pnTextLogoShapeColor.BackColor

        textLogo.TransparencyLevel = tbTextLogoTransp.Value

        If (rbTextLogoDrawText.Checked) Then
            textLogo.Mode = TextLogoMode.Text
        ElseIf (rbTextLogoDrawDate.Checked) Then
            textLogo.Mode = TextLogoMode.DateTime
            textLogo.DateTimeMask = "yyyy-MM-dd. hh:mm:ss"
        ElseIf (rbTextLogoDrawFrameNumber.Checked) Then
            textLogo.Mode = TextLogoMode.FrameNumber
        Else
            textLogo.Mode = TextLogoMode.Timestamp
        End If

        textLogo.Update()

    End Sub

    Private Sub pnTextLogoBGColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnTextLogoBGColor.Click

        colorDialog1.Color = pnTextLogoBGColor.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnTextLogoBGColor.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub pnTextLogoGradColor1_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnTextLogoGradColor1.Click

        colorDialog1.Color = pnTextLogoGradColor1.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnTextLogoGradColor1.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub pnTextLogoGradColor2_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnTextLogoGradColor2.Click

        colorDialog1.Color = pnTextLogoGradColor2.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnTextLogoGradColor2.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub pnTextLogoInnerColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnTextLogoInnerColor.Click

        colorDialog1.Color = pnTextLogoInnerColor.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnTextLogoInnerColor.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub pnTextLogoOuterColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnTextLogoOuterColor.Click

        colorDialog1.Color = pnTextLogoOuterColor.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnTextLogoOuterColor.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub pnTextLogoShapeColor_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnTextLogoShapeColor.Click

        colorDialog1.Color = pnTextLogoShapeColor.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnTextLogoShapeColor.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub cbScreenFlipVertical_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbScreenFlipVertical.CheckedChanged

        VideoCapture1.Video_Renderer.Flip_Vertical = cbScreenFlipVertical.Checked
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub cbScreenFlipHorizontal_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbScreenFlipHorizontal.CheckedChanged

        VideoCapture1.Video_Renderer.Flip_Horizontal = cbScreenFlipHorizontal.Checked
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub btMotDetUpdateSettings_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btMotDetUpdateSettings.Click

        If (cbMotDetEnabled.Checked) Then

            VideoCapture1.Motion_Detection = New MotionDetectionSettings()
            VideoCapture1.Motion_Detection.Enabled = cbMotDetEnabled.Checked
            VideoCapture1.Motion_Detection.Compare_Red = cbCompareRed.Checked
            VideoCapture1.Motion_Detection.Compare_Green = cbCompareGreen.Checked
            VideoCapture1.Motion_Detection.Compare_Blue = cbCompareBlue.Checked
            VideoCapture1.Motion_Detection.Compare_Greyscale = cbCompareGreyscale.Checked
            VideoCapture1.Motion_Detection.Highlight_Color = cbMotDetHLColor.SelectedIndex
            VideoCapture1.Motion_Detection.Highlight_Enabled = cbMotDetHLEnabled.Checked
            VideoCapture1.Motion_Detection.Highlight_Threshold = tbMotDetHLThreshold.Value
            VideoCapture1.Motion_Detection.FrameInterval = Convert.ToInt32(edMotDetFrameInterval.Text)
            VideoCapture1.Motion_Detection.Matrix_Height = Convert.ToInt32(edMotDetMatrixHeight.Text)
            VideoCapture1.Motion_Detection.Matrix_Width = Convert.ToInt32(edMotDetMatrixWidth.Text)
            VideoCapture1.Motion_Detection.DropFrames_Enabled = cbMotDetDropFramesEnabled.Checked
            VideoCapture1.Motion_Detection.DropFrames_Threshold = tbMotDetDropFramesThreshold.Value
            VideoCapture1.MotionDetection_Update()

        Else

            VideoCapture1.Motion_Detection = Nothing

        End If

    End Sub

    Private Sub btSignal_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btSignal.Click

        If VideoCapture1.Video_CaptureDevice_SignalPresent() Then
            MessageBox.Show("Signal present")
        Else
            MessageBox.Show("Signal not present")
        End If

    End Sub

    Private Sub cbStretch1_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbStretch1.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(0, cbStretch1.Checked, cbFlipHorizontal1.Checked, cbFlipVertical1.Checked)

    End Sub

    Private Sub cbFlipVertical1_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFlipVertical1.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(0, cbStretch1.Checked, cbFlipHorizontal1.Checked, cbFlipVertical1.Checked)

    End Sub

    Private Sub cbFlipHorizontal1_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFlipHorizontal1.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(0, cbStretch1.Checked, cbFlipHorizontal1.Checked, cbFlipVertical1.Checked)

    End Sub

    Private Sub cbStretch2_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbStretch2.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(1, cbStretch2.Checked, cbFlipHorizontal2.Checked, cbFlipVertical2.Checked)

    End Sub

    Private Sub cbFlipVertical2_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFlipVertical2.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(1, cbStretch2.Checked, cbFlipHorizontal2.Checked, cbFlipVertical2.Checked)

    End Sub

    Private Sub cbFlipHorizontal2_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFlipHorizontal2.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(1, cbStretch2.Checked, cbFlipHorizontal2.Checked, cbFlipVertical2.Checked)

    End Sub

    Private Sub cbStretch3_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbStretch3.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(2, cbStretch3.Checked, cbFlipHorizontal3.Checked, cbFlipVertical3.Checked)

    End Sub

    Private Sub cbFlipVertical3_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFlipVertical3.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(2, cbStretch3.Checked, cbFlipHorizontal3.Checked, cbFlipVertical3.Checked)

    End Sub

    Private Sub cbFlipHorizontal3_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbFlipHorizontal3.CheckedChanged

        VideoCapture1.MultiScreen_SetParameters(2, cbStretch3.Checked, cbFlipHorizontal3.Checked, cbFlipVertical3.Checked)

    End Sub

    Private Sub cbWMVVideoSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbWMVVideoMode.SelectedIndexChanged

        Dim mode = VFWMVStreamMode.CBR
        Select Case (cbWMVVideoMode.SelectedIndex)
            Case 0
                mode = VFWMVStreamMode.CBR
                edWMVVideoBitrate.Enabled = True
                edWMVVideoPeakBitrate.Enabled = False
                edWMVVideoQuality.Enabled = False
            Case 1
                mode = VFWMVStreamMode.VBRBitrate
                edWMVVideoBitrate.Enabled = True
                edWMVVideoPeakBitrate.Enabled = False
                edWMVVideoQuality.Enabled = False
            Case 2
                mode = VFWMVStreamMode.VBRPeakBitrate
                edWMVVideoBitrate.Enabled = True
                edWMVVideoPeakBitrate.Enabled = True
                edWMVVideoQuality.Enabled = False
            Case 3
                mode = VFWMVStreamMode.VBRQuality
                edWMVVideoBitrate.Enabled = False
                edWMVVideoPeakBitrate.Enabled = False
                edWMVVideoQuality.Enabled = True
        End Select

        cbWMVVideoCodec.Items.Clear()
        Dim codecs = VideoCapture1.WMV_CustomProfile_VideoCodecs(mode)
        For i As Integer = 0 To codecs.Count - 1
            cbWMVVideoCodec.Items.Add(codecs.Item(i))
        Next i

    End Sub

    Private Sub cbWMVAudioCodec_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbWMVAudioCodec.SelectedIndexChanged

        Dim mode = VFWMVStreamMode.CBR
        Select Case (cbWMVAudioMode.SelectedIndex)
            Case 0 : mode = VFWMVStreamMode.CBR
            Case 1 : mode = VFWMVStreamMode.VBRBitrate
            Case 2 : mode = VFWMVStreamMode.VBRPeakBitrate
            Case 3 : mode = VFWMVStreamMode.VBRQuality
        End Select

        cbWMVAudioFormat.Items.Clear()

        If cbWMVAudioCodec.SelectedIndex <> -1 Then

            Dim list As List(Of String)
            list = VideoCapture1.WMV_CustomProfile_AudioFormats(cbWMVAudioCodec.Text, mode)

            For i As Integer = 0 To list.Count - 1
                cbWMVAudioFormat.Items.Add(list.Item(i))
            Next i

        End If

    End Sub


    Private Sub tbLightness_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbLightness.Scroll

        Dim intf As IVFVideoEffectLightness
        Dim effect = VideoCapture1.Video_Effects_Get("Lightness")
        If (IsNothing(effect)) Then
            intf = New VFVideoEffectLightness(True, tbLightness.Value)
            VideoCapture1.Video_Effects_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbLightness.Value
            End If
        End If

    End Sub

    Private Sub tbSaturation_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbSaturation.Scroll

        Dim intf As IVFVideoEffectSaturation
        Dim effect = VideoCapture1.Video_Effects_Get("Saturation")
        If (IsNothing(effect)) Then
            intf = New VFVideoEffectSaturation(tbSaturation.Value)
            VideoCapture1.Video_Effects_Add(intf)
        Else

            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbSaturation.Value
            End If
        End If

    End Sub

    Private Sub tbContrast_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbContrast.Scroll

        Dim intf As IVFVideoEffectContrast
        Dim effect = VideoCapture1.Video_Effects_Get("Contrast")
        If (IsNothing(effect)) Then
            intf = New VFVideoEffectContrast(True, tbContrast.Value)
            VideoCapture1.Video_Effects_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbContrast.Value
            End If
        End If

    End Sub

    Private Sub tbDarkness_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbDarkness.Scroll

        Dim intf As IVFVideoEffectDarkness
        Dim effect = VideoCapture1.Video_Effects_Get("Darkness")
        If (IsNothing(effect)) Then
            intf = New VFVideoEffectDarkness(True, tbDarkness.Value)
            VideoCapture1.Video_Effects_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbDarkness.Value
            End If
        End If

    End Sub

    Private Sub cbAudioEffectsEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudioEffectsEnabled.CheckedChanged

        VideoCapture1.Audio_Effects_Enable(-1, 1, cbAudEqualizerEnabled.Checked)

    End Sub

    Private Sub pnOSDColorKey_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles pnOSDColorKey.Click

        If colorDialog1.ShowDialog() = DialogResult.OK Then
            pnOSDColorKey.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub btAudEqRefresh_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btAudEqRefresh.Click

        tbAudEq0.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 0)
        tbAudEq1.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 1)
        tbAudEq2.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 2)
        tbAudEq3.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 3)
        tbAudEq4.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 4)
        tbAudEq5.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 5)
        tbAudEq6.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 6)
        tbAudEq7.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 7)
        tbAudEq8.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 8)
        tbAudEq9.Value = VideoCapture1.Audio_Effects_Equalizer_Band_Get(-1, 1, 9)

    End Sub

    Private Sub btZoomIn_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btZoomIn.Click

        VideoCapture1.Video_Renderer.Zoom_Ratio = VideoCapture1.Video_Renderer.Zoom_Ratio + 10
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub btZoomOut_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btZoomOut.Click

        VideoCapture1.Video_Renderer.Zoom_Ratio = VideoCapture1.Video_Renderer.Zoom_Ratio - 10
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub btZoomShiftDown_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btZoomShiftDown.Click

        VideoCapture1.Video_Renderer.Zoom_ShiftY = VideoCapture1.Video_Renderer.Zoom_ShiftY - 10
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub btZoomShiftLeft_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btZoomShiftLeft.Click

        VideoCapture1.Video_Renderer.Zoom_ShiftX = VideoCapture1.Video_Renderer.Zoom_ShiftX - 10
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub btZoomShiftRight_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btZoomShiftRight.Click

        VideoCapture1.Video_Renderer.Zoom_ShiftX = VideoCapture1.Video_Renderer.Zoom_ShiftX + 10
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub btZoomShiftUp_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btZoomShiftUp.Click

        VideoCapture1.Video_Renderer.Zoom_ShiftY = VideoCapture1.Video_Renderer.Zoom_ShiftY + 10
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub cbAudAmplifyEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudAmplifyEnabled.CheckedChanged

        VideoCapture1.Audio_Effects_Enable(-1, 0, cbAudAmplifyEnabled.Checked)

    End Sub

    Private Sub cbAudDynamicAmplifyEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudDynamicAmplifyEnabled.CheckedChanged

        VideoCapture1.Audio_Effects_Enable(-1, 2, cbAudDynamicAmplifyEnabled.Checked)

    End Sub

    Private Sub tbAud3DSound_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAud3DSound.Scroll

        VideoCapture1.Audio_Effects_Sound3D(-1, 3, tbAud3DSound.Value)

    End Sub

    Private Sub tbAudAmplifyAmp_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudAmplifyAmp.Scroll

        VideoCapture1.Audio_Effects_Amplify(-1, 0, tbAudAmplifyAmp.Value * 10, False)

    End Sub

    Private Sub tbAudAttack_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudAttack.Scroll

        VideoCapture1.Audio_Effects_DynamicAmplify(-1, 2, tbAudAttack.Value, tbAudDynAmp.Value, tbAudRelease.Value)

    End Sub

    Private Sub tbAudEq0_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq0.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 0, tbAudEq0.Value)

    End Sub

    Private Sub tbAudEq1_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq1.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 1, tbAudEq1.Value)

    End Sub

    Private Sub tbAudEq2_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq2.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 2, tbAudEq2.Value)

    End Sub

    Private Sub tbAudEq3_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq3.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 3, tbAudEq3.Value)

    End Sub

    Private Sub tbAudEq4_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq4.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 4, tbAudEq4.Value)

    End Sub

    Private Sub tbAudEq5_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq5.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 5, tbAudEq5.Value)

    End Sub

    Private Sub tbAudEq6_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq6.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 6, tbAudEq6.Value)

    End Sub

    Private Sub tbAudEq7_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq7.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 7, tbAudEq7.Value)

    End Sub

    Private Sub tbAudEq8_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq8.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 8, tbAudEq8.Value)

    End Sub

    Private Sub tbAudEq9_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudEq9.Scroll

        VideoCapture1.Audio_Effects_Equalizer_Band_Set(-1, 1, 9, tbAudEq9.Value)

    End Sub

    Private Sub tbAudTrueBass_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudTrueBass.Scroll

        VideoCapture1.Audio_Effects_TrueBass(-1, 4, 200, False, tbAudTrueBass.Value)

    End Sub

    Private Sub tbAudRelease_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudRelease.Scroll

        VideoCapture1.Audio_Effects_DynamicAmplify(-1, 2, tbAudAttack.Value, tbAudDynAmp.Value, tbAudRelease.Value)

    End Sub

    Private Sub cbAudEqualizerPreset_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudEqualizerPreset.SelectedIndexChanged

        VideoCapture1.Audio_Effects_Equalizer_Preset_Set(-1, 1, cbAudEqualizerPreset.SelectedIndex)
        btAudEqRefresh_Click(sender, e)

    End Sub

    Private Sub cbAudSound3DEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudSound3DEnabled.CheckedChanged

        VideoCapture1.Audio_Effects_Enable(-1, 3, cbAudSound3DEnabled.Checked)

    End Sub

    Private Sub cbAudTrueBassEnabled_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbAudTrueBassEnabled.CheckedChanged

        VideoCapture1.Audio_Effects_Enable(-1, 4, cbAudTrueBassEnabled.Checked)

    End Sub

    Private Sub cbWMVAudioSelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbWMVAudioMode.SelectedIndexChanged

        Dim mode = VFWMVStreamMode.CBR
        Select Case (cbWMVAudioMode.SelectedIndex)
            Case 0 : mode = VFWMVStreamMode.CBR
            Case 1 : mode = VFWMVStreamMode.VBRBitrate
            Case 2 : mode = VFWMVStreamMode.VBRPeakBitrate
            Case 3 : mode = VFWMVStreamMode.VBRQuality
        End Select

        cbWMVAudioCodec.Items.Clear()
        Dim codecs = VideoCapture1.WMV_CustomProfile_AudioCodecs(mode)
        For i As Integer = 0 To codecs.Count - 1
            cbWMVAudioCodec.Items.Add(codecs.Item(i))
        Next i

        cbWMVAudioCodec_SelectedIndexChanged(sender, e)

    End Sub

    Private Sub tbAudDynAmp_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbAudDynAmp.Scroll

        VideoCapture1.Audio_Effects_Amplify(-1, 0, tbAudAmplifyAmp.Value * 10, False)

    End Sub


    Private Sub tbChromaKeyContrastLow_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbChromaKeyContrastLow.Scroll

        If Not IsNothing(VideoCapture1.ChromaKey) Then
            VideoCapture1.ChromaKey.ContrastLow = tbChromaKeyContrastLow.Value
        End If

    End Sub

    Private Sub tbChromaKeyContrastHigh_Scroll(ByVal sender As System.Object, ByVal e As EventArgs) Handles tbChromaKeyContrastHigh.Scroll

        If Not IsNothing(VideoCapture1.ChromaKey) Then
            VideoCapture1.ChromaKey.ContrastHigh = tbChromaKeyContrastHigh.Value
        End If

    End Sub

    Private Sub btChromaKeySelectBGImage_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles btChromaKeySelectBGImage.Click

        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            edChromaKeyImage.Text = openFileDialog1.FileName
        End If

    End Sub

    Private Sub llVideoTutorials_LinkClicked(ByVal sender As System.Object, ByVal e As Windows.Forms.LinkLabelLinkClickedEventArgs) Handles linkLabel1.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "http://www.visioforge.com/video_tutorials")
        Process.Start(startInfo)

    End Sub

    Private Sub VideoCapture1_OnTVTunerTuneChannels(ByVal sender As System.Object, ByVal e As VisioForge.Types.TVTunerTuneChannelsEventArgs) Handles VideoCapture1.OnTVTunerTuneChannels

        Application.DoEvents()

        pbChannels.Value = e.Progress

        If e.SignalPresent Then
            cbTVChannel.Items.Add(e.Channel.ToString())

            If e.Channel = -1 Then

                pbChannels.Value = 0
                MessageBox.Show("AutoTune complete")

            End If
        End If

        Application.DoEvents()

    End Sub

    Public Delegate Sub MotionDelegate(ByVal e As MotionDetectionEventArgs)

    Public Sub MotionDelegateMethod(ByVal e As MotionDetectionEventArgs)

        Dim s As String = String.Empty

        For Each b As Byte In e.Matrix

            s += b + " "

        Next

        mmMotDetMatrix.Text = s.Trim()
        pbMotionLevel.Value = e.Level

    End Sub

    Private Sub VideoCapture1_OnMotion(ByVal sender As System.Object, ByVal e As MotionDetectionEventArgs) Handles VideoCapture1.OnMotion

        Dim motdel As MotionDelegate = New MotionDelegate(AddressOf MotionDelegateMethod)
        BeginInvoke(motdel, e)

    End Sub

    Private Sub VideoCapture1_OnError(ByVal sender As System.Object, ByVal e As VisioForge.Types.ErrorsEventArgs) Handles VideoCapture1.OnError

        mmLog.Text = mmLog.Text + e.Message + Environment.NewLine

    End Sub

    Private Delegate Sub VUDelegate(ByVal e As VisioForge.Types.VUMeterEventArgs)

    Private Sub VUDelegateMethod(ByVal e As VisioForge.Types.VUMeterEventArgs)

        peakMeterCtrl1.SetData(e.MeterData, 0, 19)

    End Sub

    Private Sub VideoCapture1_OnAudioVUMeter(ByVal sender As System.Object, ByVal e As VisioForge.Types.VUMeterEventArgs) Handles VideoCapture1.OnAudioVUMeter

        BeginInvoke(New VUDelegate(AddressOf VUDelegateMethod), e)

    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub cbMPEGEncoder_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbMPEGEncoder.SelectedIndexChanged

        If cbMPEGEncoder.SelectedIndex <> -1 Then
            VideoCapture1.Video_CaptureDevice_InternalMPEGEncoder_Name = cbMPEGEncoder.Text
        End If

    End Sub

    Private Sub VideoCapture1_OnObjectDetection(ByVal sender As System.Object, ByVal e As VisioForge.Types.MotionDetectionExEventArgs) Handles VideoCapture1.OnMotionDetectionEx

        Dim motdel As AFMotionDelegate = New AFMotionDelegate(AddressOf AFMotionDelegateMethod)
        BeginInvoke(motdel, e.Level)

    End Sub

    Public Delegate Sub AFMotionDelegate(ByVal level As System.Single)

    Public Sub AFMotionDelegateMethod(ByVal level As System.Single)

        pbAFMotionLevel.Value = level * 100

    End Sub

    Private Sub cbAFMotionDetection_CheckedChanged(ByVal sender As System.Object, ByVal e As EventArgs) Handles cbMotionDetectionEx.CheckedChanged

        ConfigureMotionDetectionEx()

    End Sub

    Private Sub btSeparateCaptureStart_Click(sender As System.Object, e As EventArgs) Handles btSeparateCaptureStart.Click

        VideoCapture1.SeparateCapture_Start(edOutput.Text)

    End Sub

    Private Sub btSeparateCaptureStop_Click(sender As System.Object, e As EventArgs) Handles btSeparateCaptureStop.Click

        VideoCapture1.SeparateCapture_Stop()

    End Sub

    Private Sub btSeparateCaptureChangeFilename_Click(sender As System.Object, e As EventArgs) Handles btSeparateCaptureChangeFilename.Click

        VideoCapture1.SeparateCapture_ChangeFilenameOnTheFly(edNewFilename.Text)

    End Sub


    Private Sub btPIPAddIPCamera_Click(sender As System.Object, e As EventArgs) Handles btPIPAddIPCamera.Click

        Dim ipCameraSource As IPCameraSourceSettings = SelectIPCameraSource()

        VideoCapture1.PIP_Sources_Add_IPCamera(
            ipCameraSource,
            Convert.ToInt32(edPIPScreenCapLeft.Text),
            Convert.ToInt32(edPIPScreenCapTop.Text),
            Convert.ToInt32(edPIPScreenCapWidth.Text),
            Convert.ToInt32(edPIPScreenCapHeight.Text))

        cbPIPDevices.Items.Add("IP Capture")

    End Sub

    Private Sub btPIPAddScreenCapture_Click(sender As System.Object, e As EventArgs) Handles btPIPAddScreenCapture.Click

        Dim screenSource As ScreenCaptureSourceSettings = SelectScreenSource()

        VideoCapture1.PIP_Sources_Add_ScreenSource(
            screenSource,
            Convert.ToInt32(edPIPScreenCapLeft.Text),
            Convert.ToInt32(edPIPScreenCapTop.Text),
            Convert.ToInt32(edPIPScreenCapWidth.Text),
            Convert.ToInt32(edPIPScreenCapHeight.Text))

        cbPIPDevices.Items.Add("Screen Capture")

    End Sub

    Private Sub btPIPDevicesClear_Click(sender As System.Object, e As EventArgs) Handles btPIPDevicesClear.Click

        VideoCapture1.PIP_Sources_Clear()
        cbPIPDevices.Items.Clear()

    End Sub

    Private Sub btPIPUpdate_Click(sender As System.Object, e As EventArgs) Handles btPIPUpdate.Click

        If (cbPIPDevices.SelectedIndex <> -1) Then
            VideoCapture1.PIP_Sources_SetSourcePosition(
                cbPIPDevices.SelectedIndex,
                Convert.ToInt32(edPIPLeft.Text),
                Convert.ToInt32(edPIPTop.Text),
                Convert.ToInt32(edPIPWidth.Text),
                Convert.ToInt32(edPIPHeight.Text))
        Else
            MessageBox.Show("Select device!")
        End If

    End Sub

    Private Sub btPIPSetOutputSize_Click(sender As System.Object, e As EventArgs) Handles btPIPSetOutputSize.Click

        VideoCapture1.PIP_CustomOutputSize_Set(Convert.ToInt32(edPIPOutputWidth.Text), Convert.ToInt32(edPIPOutputHeight.Text))

    End Sub

    Private Sub btPIPSet_Click(sender As System.Object, e As EventArgs) Handles btPIPSet.Click

        If (cbPIPDevices.SelectedIndex <> -1) Then
            VideoCapture1.PIP_Sources_SetSourceSettings(cbPIPDevices.SelectedIndex, tbPIPTransparency.Value, False, False)
        Else
            MessageBox.Show("Select device!")
        End If

    End Sub

    Private Sub btSeparateCapturePause_Click(sender As System.Object, e As EventArgs) Handles btSeparateCapturePause.Click

        VideoCapture1.SeparateCapture_Pause()

    End Sub

    Private Sub btSeparateCaptureResume_Click(sender As System.Object, e As EventArgs) Handles btSeparateCaptureResume.Click

        VideoCapture1.SeparateCapture_Resume()

    End Sub

    Private Sub btDVBTTune_Click(sender As System.Object, e As EventArgs) Handles btDVBTTune.Click

        If (Not IsNothing(VideoCapture1.BDA_Source) And VideoCapture1.BDA_Source.SourceType = VFBDAType.DVBT) Then

            Dim bdaTuningParameters As VFBDATuningParameters = New VFBDATuningParameters

            bdaTuningParameters.Frequency = Convert.ToInt32(edDVBTFrequency.Text)
            bdaTuningParameters.ONID = Convert.ToInt32(edDVBTONID.Text)
            bdaTuningParameters.SID = Convert.ToInt32(edDVBTSID.Text)
            bdaTuningParameters.TSID = Convert.ToInt32(edDVBTTSID.Text)

            VideoCapture1.BDA_SetParameters(bdaTuningParameters)

        End If

    End Sub

    Private Sub cbZoom_CheckedChanged(sender As Object, e As EventArgs) Handles cbZoom.CheckedChanged

        Dim zoomEffect As IVFVideoEffectZoom
        Dim effect = VideoCapture1.Video_Effects_Get("Zoom")
        If (IsNothing(effect)) Then
            zoomEffect = New VFVideoEffectZoom(zoom, zoom, zoomShiftX, zoomShiftY, cbZoom.Checked)
            VideoCapture1.Video_Effects_Add(zoomEffect)
        Else
            zoomEffect = effect
        End If

        If (IsNothing(zoomEffect)) Then
            MessageBox.Show("Unable to configure zoom effect.")
            Return
        End If

        zoomEffect.ZoomX = zoom
        zoomEffect.ZoomY = zoom
        zoomEffect.ShiftX = zoomShiftX
        zoomEffect.ShiftY = zoomShiftY
        zoomEffect.Enabled = cbZoom.Checked

    End Sub


    Private Sub btEffZoomIn_Click(sender As Object, e As EventArgs) Handles btEffZoomIn.Click

        zoom += 0.1
        zoom = Math.Min(zoom, 5)

        cbZoom_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub btEffZoomOut_Click(sender As Object, e As EventArgs) Handles btEffZoomOut.Click

        zoom -= 0.1
        zoom = Math.Max(zoom, 1)

        cbZoom_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub btEffZoomUp_Click(sender As Object, e As EventArgs) Handles btEffZoomUp.Click

        zoomShiftY += 20

        cbZoom_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub btEffZoomDown_Click(sender As Object, e As EventArgs) Handles btEffZoomDown.Click

        zoomShiftY -= 20

        cbZoom_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub btEffZoomRight_Click(sender As Object, e As EventArgs) Handles btEffZoomRight.Click

        zoomShiftX += 20

        cbZoom_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub btEffZoomLeft_Click(sender As Object, e As EventArgs) Handles btEffZoomLeft.Click

        zoomShiftX -= 20

        cbZoom_CheckedChanged(Nothing, Nothing)

    End Sub

    Private Sub cbPan_CheckedChanged(sender As Object, e As EventArgs) Handles cbPan.CheckedChanged

        Dim pan As IVFVideoEffectPan
        Dim effect = VideoCapture1.Video_Effects_Get("Pan")
        If (IsNothing(effect)) Then
            pan = New VFVideoEffectPan(True)
            VideoCapture1.Video_Effects_Add(pan)
        Else
            pan = effect
        End If

        If (IsNothing(pan)) Then
            MessageBox.Show("Unable to configure pan effect.")
            Return
        End If

        pan.Enabled = cbPan.Checked
        pan.StartTime = Convert.ToInt32(edPanStartTime.Text)
        pan.StopTime = Convert.ToInt32(edPanStopTime.Text)
        pan.StartX = Convert.ToInt32(edPanSourceLeft.Text)
        pan.StartY = Convert.ToInt32(edPanSourceTop.Text)
        pan.StartWidth = Convert.ToInt32(edPanSourceWidth.Text)
        pan.StartHeight = Convert.ToInt32(edPanSourceHeight.Text)
        pan.StopX = Convert.ToInt32(edPanDestLeft.Text)
        pan.StopY = Convert.ToInt32(edPanDestTop.Text)
        pan.StopWidth = Convert.ToInt32(edPanDestWidth.Text)
        pan.StopHeight = Convert.ToInt32(edPanDestHeight.Text)

    End Sub

    Private Sub btBarcodeReset_Click(sender As Object, e As EventArgs) Handles btBarcodeReset.Click

        edBarcode.Text = String.Empty
        edBarcodeMetadata.Text = String.Empty
        VideoCapture1.Barcode_Reader_Enabled = True

    End Sub

    Private Sub VideoCapture1_OnBarcodeDetected(sender As Object, e As BarcodeEventArgs) Handles VideoCapture1.OnBarcodeDetected

        e.DetectorEnabled = False

        BeginInvoke(New BarcodeDelegate(AddressOf BarcodeDelegateMethod), e)

    End Sub

    Private Delegate Sub BarcodeDelegate(ByVal value As BarcodeEventArgs)

    Private Sub BarcodeDelegateMethod(ByVal value As BarcodeEventArgs)

        edBarcode.Text = value.Value
        edBarcodeMetadata.Text = String.Empty

        For Each o As KeyValuePair(Of VFBarcodeResultMetadataType, Object) In value.Metadata

            edBarcodeMetadata.Text += o.Key.ToString() + ": " + o.Value.ToString() + Environment.NewLine

        Next

    End Sub

    Private Sub btAddAdditionalAudioSource_Click(sender As Object, e As EventArgs) Handles btAddAdditionalAudioSource.Click

        VideoCapture1.Additional_Audio_CaptureDevice_Add(cbAdditionalAudioSource.Text)
        MessageBox.Show(cbAdditionalAudioSource.Text + " added")

    End Sub

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click

        If (openFileDialog1.ShowDialog() = DialogResult.OK) Then

            edPIPFileSoureFilename.Text = openFileDialog1.FileName

        End If

    End Sub

    Private Sub btPIPFileSourceAdd_Click(sender As Object, e As EventArgs) Handles btPIPFileSourceAdd.Click

        VideoCapture1.PIP_Sources_Add_VideoFile(
    edPIPFileSoureFilename.Text,
    Convert.ToInt32(edPIPFileLeft.Text),
    Convert.ToInt32(edPIPFileTop.Text),
    Convert.ToInt32(edPIPFileWidth.Text),
    Convert.ToInt32(edPIPFileHeight.Text))
        cbPIPDevices.Items.Add("File source")

    End Sub

    Private Sub cbFadeInOut_CheckedChanged(sender As Object, e As EventArgs) Handles cbFadeInOut.CheckedChanged

        If (rbFadeIn.Checked) Then
            Dim fadeIn As IVFVideoEffectFadeIn
            Dim effect = VideoCapture1.Video_Effects_Get("FadeIn")
            If (IsNothing(effect)) Then
                fadeIn = New VFVideoEffectFadeIn(cbFadeInOut.Checked)
                VideoCapture1.Video_Effects_Add(fadeIn)
            Else
                fadeIn = effect
            End If

            If (IsNothing(fadeIn)) Then
                MessageBox.Show("Unable to configure fade-in effect.")
                Return
            End If

            fadeIn.Enabled = cbFadeInOut.Checked
            fadeIn.StartTime = Convert.ToInt64(edFadeInOutStartTime.Text)
            fadeIn.StopTime = Convert.ToInt64(edFadeInOutStopTime.Text)
        Else
            Dim fadeOut As IVFVideoEffectFadeOut
            Dim effect = VideoCapture1.Video_Effects_Get("FadeOut")
            If (IsNothing(effect)) Then
                fadeOut = New VFVideoEffectFadeOut(cbFadeInOut.Checked)
                VideoCapture1.Video_Effects_Add(fadeOut)
            Else
                fadeOut = effect
            End If

            If (IsNothing(fadeOut)) Then
                MessageBox.Show("Unable to configure fade-out effect.")
                Return
            End If

            fadeOut.Enabled = cbFadeInOut.Checked
            fadeOut.StartTime = Convert.ToInt64(edFadeInOutStartTime.Text)
            fadeOut.StopTime = Convert.ToInt64(edFadeInOutStopTime.Text)
        End If

    End Sub

    Private Sub linkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel2.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "http://www.visioforge.com/support/878966-Streaming-to-Adobe-Flash-Media-Server")
        Process.Start(startInfo)

    End Sub

    Private Sub btBDAChannelScanningStart_Click(sender As Object, e As EventArgs) Handles btBDAChannelScanningStart.Click

        lvBDAChannels.Items.Clear()
        VideoCapture1.BDA_ScanChannels()

    End Sub

    Private Sub VideoCapture1_OnBDAChannelFound(sender As Object, e As BDAChannelEventArgs) Handles VideoCapture1.OnBDAChannelFound

        Application.DoEvents()

        Dim list As String() = New String() {
            e.Channel.Name,
        e.Channel.Frequency.ToString(CultureInfo.InvariantCulture),
        e.Channel.AudioPid.ToString(CultureInfo.InvariantCulture),
        e.Channel.VideoPid.ToString(CultureInfo.InvariantCulture),
        e.Channel.ServId.ToString(CultureInfo.InvariantCulture),
        e.Channel.SymbolRate.ToString(CultureInfo.InvariantCulture)
                                            }

        Dim lvi As ListViewItem = New ListViewItem(list)

        lvBDAChannels.Items.Add(lvi)

        Application.DoEvents()
    End Sub

    Private Sub linkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel4.LinkClicked

        Dim startInfo As ProcessStartInfo = New ProcessStartInfo("explorer.exe", "http://www.visioforge.com/support/300487-Streaming-using-Microsoft-Expression-Encoder")
        Process.Start(startInfo)

    End Sub

    Private Sub VideoCapture1_OnFaceDetected(sender As Object, e As AFFaceDetectionEventArgs) Handles VideoCapture1.OnFaceDetected

        BeginInvoke(New FaceDelegate(AddressOf FaceDelegateMethod), e)

    End Sub

    Private Delegate Sub FaceDelegate(ByVal value As AFFaceDetectionEventArgs)

    Private Sub FaceDelegateMethod(ByVal value As AFFaceDetectionEventArgs)

        edFaceTrackingFaces.Text = String.Empty

        For Each faceRectangle As Rectangle In value.FaceRectangles

            edFaceTrackingFaces.Text += "(" + faceRectangle.Left + ", " + faceRectangle.Top + "), (" + faceRectangle.Width + ", " + faceRectangle.Height + ")" + Environment.NewLine
        Next

    End Sub


#Region "Full screen"

    Dim fullScreen As Boolean

    Dim windowLeft As Integer

    Dim windowTop As Integer

    Dim windowWidth As Integer

    Dim windowHeight As Integer

    Dim controlLeft As Integer

    Dim controlTop As Integer

    Dim controlWidth As Integer

    Dim controlHeight As Integer

    Private Sub btFullScreen_Click(sender As Object, e As EventArgs) Handles btFullScreen.Click

        If (Not fullScreen) Then

            ' going fullscreen
            fullScreen = True

            ' saving coordinates
            windowLeft = Left
            windowTop = Top
            windowWidth = Width
            windowHeight = Height

            controlLeft = VideoCapture1.Left
            controlTop = VideoCapture1.Top
            controlWidth = VideoCapture1.Width
            controlHeight = VideoCapture1.Height

            ' resizing window
            Left = 0
            Top = 0
            Width = Screen.AllScreens(0).WorkingArea.Width
            Height = Screen.AllScreens(0).WorkingArea.Height

            TopMost = True
            FormBorderStyle = FormBorderStyle.None
            WindowState = FormWindowState.Maximized

            ' resizing control
            VideoCapture1.Left = 0
            VideoCapture1.Top = 0
            VideoCapture1.Width = Width
            VideoCapture1.Height = Height

            VideoCapture1.Video_Renderer_Update()

        Else
            ' going normal screen
            fullScreen = False

            ' restoring control
            VideoCapture1.Left = controlLeft
            VideoCapture1.Top = controlTop
            VideoCapture1.Width = controlWidth
            VideoCapture1.Height = controlHeight

            ' restoring window
            Left = windowLeft
            Top = windowTop
            Width = windowWidth
            Height = windowHeight

            TopMost = False
            FormBorderStyle = FormBorderStyle.Sizable
            WindowState = FormWindowState.Normal

            VideoCapture1.Video_Renderer_Update()

        End If

    End Sub

    Private Sub VideoCapture1_MouseDown(sender As Object, e As MouseEventArgs) Handles VideoCapture1.MouseDown

        If (fullScreen) Then

            btFullScreen_Click(sender, e)

        End If

    End Sub

#End Region


    Private Sub linkLabel5_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel5.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "http://www.visioforge.com/support/240078-How-to-configure-IIS-Smooth-Streaming-in-SDK-demo-application")
        Process.Start(startInfo)

    End Sub

    Private Sub tbVUMeterAmplification_Scroll(sender As Object, e As EventArgs) Handles tbVUMeterAmplification.Scroll

        VideoCapture1.Audio_VUMeter_Pro_Volume = tbVUMeterAmplification.Value

    End Sub

    Private Sub tbVUMeterBoost_Scroll(sender As Object, e As EventArgs) Handles tbVUMeterBoost.Scroll

        volumeMeter1.Boost = tbVUMeterBoost.Value / 10.0F
        volumeMeter2.Boost = tbVUMeterBoost.Value / 10.0F

        waveformPainter1.Boost = tbVUMeterBoost.Value / 10.0F
        waveformPainter2.Boost = tbVUMeterBoost.Value / 10.0F

    End Sub

    Private Sub VideoCapture1_OnAudioVUMeterProVolume(sender As Object, e As AudioLevelEventArgs) Handles VideoCapture1.OnAudioVUMeterProVolume

        volumeMeter1.Amplitude = e.ChannelLevelsDb(0)
        waveformPainter1.AddMax(e.ChannelLevelsDb(0))

        If (e.ChannelLevelsDb.Length > 1) Then

            volumeMeter2.Amplitude = e.ChannelLevelsDb(1)
            waveformPainter2.AddMax(e.ChannelLevelsDb(1))

        End If

    End Sub

    Private Sub cbLiveRotation_CheckedChanged(sender As Object, e As EventArgs) Handles cbLiveRotation.CheckedChanged

        Dim rotate As IVFVideoEffectRotate
        Dim effect = VideoCapture1.Video_Effects_Get("Rotate")
        If (IsNothing(effect)) Then
            rotate = New VFVideoEffectRotate(
                    cbLiveRotation.Checked,
                    tbLiveRotationAngle.Value,
                    cbLiveRotationStretch.Checked)
            VideoCapture1.Video_Effects_Add(rotate)
        Else
            rotate = effect
        End If

        If (IsNothing(rotate)) Then
            MessageBox.Show("Unable to configure rotate effect.")
            Return
        End If

        rotate.Enabled = cbLiveRotation.Checked
        rotate.Angle = tbLiveRotationAngle.Value
        rotate.Stretch = cbLiveRotationStretch.Checked

    End Sub

    Private Sub tbLiveRotationAngle_Scroll(sender As Object, e As EventArgs) Handles tbLiveRotationAngle.Scroll

        cbLiveRotation_CheckedChanged(sender, e)

    End Sub

    Private Sub cbLiveRotationStretch_CheckedChanged(sender As Object, e As EventArgs) Handles cbLiveRotationStretch.CheckedChanged

        cbLiveRotation_CheckedChanged(sender, e)

    End Sub

    Private Sub cbDirect2DRotate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDirect2DRotate.SelectedIndexChanged

        VideoCapture1.Video_Renderer.RotationAngle = Convert.ToInt32(cbDirect2DRotate.Text)
        VideoCapture1.Video_Renderer_Update()

    End Sub

    Private Sub pnVideoRendererBGColor_Click(sender As Object, e As EventArgs) Handles pnVideoRendererBGColor.Click

        colorDialog1.Color = pnVideoRendererBGColor.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then

            pnVideoRendererBGColor.BackColor = colorDialog1.Color

            VideoCapture1.Video_Renderer.BackgroundColor = pnVideoRendererBGColor.BackColor
            VideoCapture1.Video_Renderer_Update()

        End If

    End Sub

    Private Sub cbCustomVideoSourceCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbCustomVideoSourceCategory.SelectedIndexChanged

        cbCustomVideoSourceFilter.Items.Clear()

        If (cbCustomVideoSourceCategory.SelectedIndex = 0) Then

            Dim filters = VideoCapture1.Video_CaptureDevicesInfo
            For Each item As VideoCaptureDeviceInfo In filters
                cbCustomVideoSourceFilter.Items.Add(item.Name)
            Next

            If (filters.Count > 0) Then
                cbCustomVideoSourceFilter.SelectedIndex = 0
            End If

        ElseIf (cbCustomVideoSourceCategory.SelectedIndex = 1) Then

            Dim filters = VideoCapture1.DirectShow_Filters
            For Each item As String In filters
                cbCustomVideoSourceFilter.Items.Add(item)
            Next

            If (filters.Count > 0) Then
                cbCustomVideoSourceFilter.SelectedIndex = 0
            End If

        End If

    End Sub

    Private Sub cbCustomAudioSourceCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbCustomAudioSourceCategory.SelectedIndexChanged

        cbCustomAudioSourceFilter.Items.Clear()
        cbCustomAudioSourceFilter.Items.Add(String.Empty)

        If (cbCustomAudioSourceCategory.SelectedIndex = 0) Then

            Dim filters = VideoCapture1.Audio_CaptureDevicesInfo
            For Each item As AudioCaptureDeviceInfo In filters
                cbCustomAudioSourceFilter.Items.Add(item.Name)
            Next

            If (filters.Count > 0) Then
                cbCustomAudioSourceFilter.SelectedIndex = 0
            End If

        ElseIf (cbCustomAudioSourceCategory.SelectedIndex = 1) Then

            Dim filters = VideoCapture1.DirectShow_Filters
            For Each item As String In filters
                cbCustomAudioSourceFilter.Items.Add(item)
            Next

            If (filters.Count > 0) Then

                cbCustomAudioSourceFilter.SelectedIndex = 0

            End If
        End If
    End Sub

    Private Sub cbCustomVideoSourceFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbCustomVideoSourceFilter.SelectedIndexChanged

        If (Not String.IsNullOrEmpty(cbCustomVideoSourceFilter.Text)) Then

            cbCustomVideoSourceFormat.Items.Clear()
            cbCustomVideoSourceFrameRate.Items.Clear()

            Dim formats As List(Of String) = Nothing
            Dim frameRates As List(Of String) = Nothing

            If (cbCustomVideoSourceCategory.SelectedIndex = 0) Then

                VideoCapture1.DirectShow_Filter_GetFormats(
                    VFFilterCategory.VideoCaptureSource,
                    cbCustomVideoSourceFilter.Text,
                    VFMediaCategory.Video,
                     formats,
                     frameRates)

            Else

                VideoCapture1.DirectShow_Filter_GetFormats(
                    VFFilterCategory.DirectShowFilters,
                    cbCustomVideoSourceFilter.Text,
                    VFMediaCategory.Video,
                     formats,
                     frameRates)

            End If

            For Each format As String In formats
                cbCustomVideoSourceFormat.Items.Add(format)
            Next

            For Each format As String In frameRates
                cbCustomVideoSourceFrameRate.Items.Add(format)
            Next

        End If

    End Sub

    Private Sub cbCustomAudioSourceFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbCustomAudioSourceFilter.SelectedIndexChanged

        If (Not String.IsNullOrEmpty(cbCustomAudioSourceFilter.Text)) Then

            cbCustomAudioSourceFormat.Items.Clear()

            Dim formats As List(Of String) = Nothing
            Dim frameRates As List(Of String) = Nothing

            If (cbCustomAudioSourceCategory.SelectedIndex = 0) Then

                VideoCapture1.DirectShow_Filter_GetFormats(
                    VFFilterCategory.AudioCaptureSource,
                    cbCustomAudioSourceFilter.Text,
                    VFMediaCategory.Audio,
                    formats,
                    frameRates)

            Else

                VideoCapture1.DirectShow_Filter_GetFormats(
                    VFFilterCategory.DirectShowFilters,
                    cbCustomAudioSourceFilter.Text,
                    VFMediaCategory.Audio,
                     formats,
                     frameRates)
            End If

            For Each format As String In formats
                cbCustomAudioSourceFormat.Items.Add(format)
            Next

        End If

    End Sub

    Private Sub cbAudioNormalize_CheckedChanged(sender As Object, e As EventArgs) Handles cbAudioNormalize.CheckedChanged

        VideoCapture1.Audio_Enhancer_Normalize(cbAudioNormalize.Checked)

    End Sub

    Private Sub cbAudioAutoGain_CheckedChanged(sender As Object, e As EventArgs) Handles cbAudioAutoGain.CheckedChanged

        VideoCapture1.Audio_Enhancer_AutoGain(cbAudioAutoGain.Checked)

    End Sub

    Private Sub ApplyAudioInputGains()

        Dim gains As VFAudioEnhancerGains = New VFAudioEnhancerGains()

        gains.L = tbAudioInputGainL.Value / 10.0F
        gains.C = tbAudioInputGainC.Value / 10.0F
        gains.R = tbAudioInputGainR.Value / 10.0F
        gains.SL = tbAudioInputGainSL.Value / 10.0F
        gains.SR = tbAudioInputGainSR.Value / 10.0F
        gains.LFE = tbAudioInputGainLFE.Value / 10.0F

        VideoCapture1.Audio_Enhancer_InputGains(gains)

    End Sub

    Private Sub ApplyAudioOutputGains()

        Dim gains As VFAudioEnhancerGains = New VFAudioEnhancerGains

        gains.L = tbAudioOutputGainL.Value / 10.0F
        gains.C = tbAudioOutputGainC.Value / 10.0F
        gains.R = tbAudioOutputGainR.Value / 10.0F
        gains.SL = tbAudioOutputGainSL.Value / 10.0F
        gains.SR = tbAudioOutputGainSR.Value / 10.0F
        gains.LFE = tbAudioOutputGainLFE.Value / 10.0F

        VideoCapture1.Audio_Enhancer_OutputGains(gains)

    End Sub

    Private Sub tbAudioInputGainL_Scroll(sender As Object, e As EventArgs) Handles tbAudioInputGainL.Scroll

        lbAudioInputGainL.Text = (tbAudioInputGainL.Value / 10.0F).ToString("F1")

        ApplyAudioInputGains()

    End Sub

    Private Sub tbAudioInputGainC_Scroll(sender As Object, e As EventArgs) Handles tbAudioInputGainC.Scroll

        lbAudioInputGainC.Text = (tbAudioInputGainC.Value / 10.0F).ToString("F1")

        ApplyAudioInputGains()

    End Sub

    Private Sub tbAudioInputGainR_Scroll(sender As Object, e As EventArgs) Handles tbAudioInputGainR.Scroll

        lbAudioInputGainR.Text = (tbAudioInputGainR.Value / 10.0F).ToString("F1")

        ApplyAudioInputGains()

    End Sub

    Private Sub tbAudioInputGainSL_Scroll(sender As Object, e As EventArgs) Handles tbAudioInputGainSL.Scroll

        lbAudioInputGainSL.Text = (tbAudioInputGainSL.Value / 10.0F).ToString("F1")

        ApplyAudioInputGains()

    End Sub

    Private Sub tbAudioInputGainSR_Scroll(sender As Object, e As EventArgs) Handles tbAudioInputGainSR.Scroll

        lbAudioInputGainSR.Text = (tbAudioInputGainSR.Value / 10.0F).ToString("F1")

        ApplyAudioInputGains()

    End Sub

    Private Sub tbAudioInputGainLFE_Scroll(sender As Object, e As EventArgs) Handles tbAudioInputGainLFE.Scroll

        lbAudioInputGainLFE.Text = (tbAudioInputGainLFE.Value / 10.0F).ToString("F1")

        ApplyAudioInputGains()

    End Sub

    Private Sub tbAudioOutputGainL_Scroll(sender As Object, e As EventArgs) Handles tbAudioOutputGainL.Scroll

        lbAudioOutputGainL.Text = (tbAudioOutputGainL.Value / 10.0F).ToString("F1")

        ApplyAudioOutputGains()

    End Sub

    Private Sub tbAudioOutputGainC_Scroll(sender As Object, e As EventArgs) Handles tbAudioOutputGainC.Scroll

        lbAudioOutputGainC.Text = (tbAudioOutputGainC.Value / 10.0F).ToString("F1")

        ApplyAudioOutputGains()

    End Sub

    Private Sub tbAudioOutputGainR_Scroll(sender As Object, e As EventArgs) Handles tbAudioOutputGainR.Scroll

        lbAudioOutputGainR.Text = (tbAudioOutputGainR.Value / 10.0F).ToString("F1")

        ApplyAudioOutputGains()

    End Sub

    Private Sub tbAudioOutputGainSL_Scroll(sender As Object, e As EventArgs) Handles tbAudioOutputGainSL.Scroll

        lbAudioOutputGainSL.Text = (tbAudioOutputGainSL.Value / 10.0F).ToString("F1")

        ApplyAudioOutputGains()

    End Sub

    Private Sub tbAudioOutputGainSR_Scroll(sender As Object, e As EventArgs) Handles tbAudioOutputGainSR.Scroll

        lbAudioOutputGainSR.Text = (tbAudioOutputGainSR.Value / 10.0F).ToString("F1")

        ApplyAudioOutputGains()

    End Sub

    Private Sub tbAudioOutputGainLFE_Scroll(sender As Object, e As EventArgs) Handles tbAudioOutputGainLFE.Scroll

        lbAudioOutputGainLFE.Text = (tbAudioOutputGainLFE.Value / 10.0F).ToString("F1")

        ApplyAudioOutputGains()

    End Sub

    Private Sub tbAudioTimeshift_Scroll(sender As Object, e As EventArgs) Handles tbAudioTimeshift.Scroll

        lbAudioTimeshift.Text = tbAudioTimeshift.Value.ToString(CultureInfo.InvariantCulture) + " ms"

        VideoCapture1.Audio_Enhancer_Timeshift(tbAudioTimeshift.Value)

    End Sub

    Private Sub VideoCapture1_OnLicenseRequired(sender As Object, e As LicenseEventArgs) Handles VideoCapture1.OnLicenseRequired

        If (cbLicensing.Checked) Then
            mmLog.Text = mmLog.Text + "LICENSING:" + Environment.NewLine + e.Message + Environment.NewLine
        End If

    End Sub

    Private Sub cbFFEXEProfile_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbFFEXEProfile.SelectedIndexChanged

        Select Case (cbFFEXEProfile.SelectedIndex)
            ' MPEG-1
            Case 0
                cbFFEXEOutputFormat.SelectedIndex = 23

                cbFFEXEVideoCodec.SelectedIndex = 12
                cbFFEXEAudioCodec.SelectedIndex = 12

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MPEG-1 VCD
            Case 1
                cbFFEXEOutputFormat.SelectedIndex = 34

                cbFFEXEVideoCodec.SelectedIndex = 12
                cbFFEXEAudioCodec.SelectedIndex = 12

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MPEG-2
            Case 2
                cbFFEXEOutputFormat.SelectedIndex = 23

                cbFFEXEVideoCodec.SelectedIndex = 13
                cbFFEXEAudioCodec.SelectedIndex = 12

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MPEG-2 SVCD
            Case 3
                cbFFEXEOutputFormat.SelectedIndex = 30

                cbFFEXEVideoCodec.SelectedIndex = 13
                cbFFEXEAudioCodec.SelectedIndex = 12

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MPEG-2 DVD
            Case 4
                cbFFEXEOutputFormat.SelectedIndex = 7

                cbFFEXEVideoCodec.SelectedIndex = 13
                cbFFEXEAudioCodec.SelectedIndex = 12

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MPEG-2 TS
            Case 5
                cbFFEXEOutputFormat.SelectedIndex = 24

                cbFFEXEVideoCodec.SelectedIndex = 13
                cbFFEXEAudioCodec.SelectedIndex = 12

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' Flash Video (FLV)
            Case 6
                cbFFEXEOutputFormat.SelectedIndex = 11

                cbFFEXEVideoCodec.SelectedIndex = 5
                cbFFEXEAudioCodec.SelectedIndex = 1

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MP4 H264 / AAC
            Case 7
                cbFFEXEOutputFormat.SelectedIndex = 22

                cbFFEXEVideoCodec.SelectedIndex = 5
                cbFFEXEAudioCodec.SelectedIndex = 1

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MP4 HEVC / AAC
            Case 8
                cbFFEXEOutputFormat.SelectedIndex = 22

                cbFFEXEVideoCodec.SelectedIndex = 6
                cbFFEXEAudioCodec.SelectedIndex = 1

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' WebM
            Case 9
                cbFFEXEOutputFormat.SelectedIndex = 36

                cbFFEXEVideoCodec.SelectedIndex = 17
                cbFFEXEAudioCodec.SelectedIndex = 41

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' AVI
            Case 10
                cbFFEXEOutputFormat.SelectedIndex = 4

                cbFFEXEVideoCodec.SelectedIndex = 14
                cbFFEXEAudioCodec.SelectedIndex = 13

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' OGG Vorbis
            Case 11
                cbFFEXEOutputFormat.SelectedIndex = 26

                cbFFEXEVideoCodec.SelectedIndex = 0
                cbFFEXEAudioCodec.SelectedIndex = 41

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' Opus
            Case 12
                cbFFEXEOutputFormat.SelectedIndex = 27

                cbFFEXEVideoCodec.SelectedIndex = 0
                cbFFEXEAudioCodec.SelectedIndex = 14

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' Speex
            Case 13
                cbFFEXEOutputFormat.SelectedIndex = 26

                cbFFEXEVideoCodec.SelectedIndex = 0
                cbFFEXEAudioCodec.SelectedIndex = 40

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' FLAC
            Case 14
                cbFFEXEOutputFormat.SelectedIndex = 10

                cbFFEXEVideoCodec.SelectedIndex = 0
                cbFFEXEAudioCodec.SelectedIndex = 10

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' MP3
            Case 15
                cbFFEXEOutputFormat.SelectedIndex = 21

                cbFFEXEVideoCodec.SelectedIndex = 0
                cbFFEXEAudioCodec.SelectedIndex = 13

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

            ' DV
            Case 16
                cbFFEXEOutputFormat.SelectedIndex = 4

                cbFFEXEVideoCodec.SelectedIndex = 1
                cbFFEXEAudioCodec.SelectedIndex = 23

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

                cbFFEXEAudioChannels.SelectedIndex = 1
                cbFFEXEAudioSampleRate.SelectedIndex = 1

            ' Custom
            Case 17
                cbFFEXEOutputFormat.SelectedIndex = 4

                cbFFEXEVideoCodec.SelectedIndex = 14
                cbFFEXEAudioCodec.SelectedIndex = 13

                cbFFEXEVideoCodec_SelectedIndexChanged(Nothing, Nothing)
                cbFFEXEAudioCodec_SelectedIndexChanged(Nothing, Nothing)

        End Select

    End Sub

    Private Sub FFEXEDisableVideoMode()

        rbFFEXEVideoModeABR.Enabled = False
        rbFFEXEVideoModeABR.Checked = False
        rbFFEXEVideoModeCBR.Enabled = False
        rbFFEXEVideoModeCBR.Checked = False
        rbFFEXEVideoModeQuality.Enabled = False
        rbFFEXEVideoModeQuality.Checked = False

        tbFFEXEVideoQuality.Enabled = False
        edFFEXEVideoTargetBitrate.Enabled = False
        edFFEXEVideoBitrateMax.Enabled = False
        edFFEXEVideoBitrateMin.Enabled = False

    End Sub

    Private Sub FFEXEEnableVideoCBR()

        rbFFEXEVideoModeCBR.Enabled = True
        rbFFEXEVideoModeCBR.Checked = True

        edFFEXEVideoTargetBitrate.Enabled = True

    End Sub

    Private Sub FFEXEEnableVideoABR()

        rbFFEXEVideoModeABR.Enabled = True
        rbFFEXEVideoModeABR.Checked = True

        edFFEXEVideoTargetBitrate.Enabled = True
        edFFEXEVideoBitrateMax.Enabled = True
        edFFEXEVideoBitrateMin.Enabled = True

    End Sub

    Private Sub FFEXEEnableVideoQuality()

        rbFFEXEVideoModeQuality.Enabled = True
        rbFFEXEVideoModeQuality.Checked = True

        tbFFEXEVideoQuality.Enabled = True

    End Sub

    Private Sub cbFFEXEVideoCodec_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbFFEXEVideoCodec.SelectedIndexChanged

        edFFEXEVBVBufferSize.Text = "0"
        edFFEXEVideoGOPSize.Text = "0"
        edFFEXEVideoBFramesCount.Text = "0"
        tbFFEXEVideoQuality.Minimum = 1
        tbFFEXEVideoQuality.Maximum = 31

        lbFFEXEVideoNotes.Text = "Notes: None."

        FFEXEDisableVideoMode()

        Select Case (cbFFEXEVideoCodec.SelectedIndex)

            ' Auto / None
            Case 0
                FFEXEEnableVideoABR()
                FFEXEEnableVideoQuality()

            ' DV
            Case 1

                lbFFEXEVideoNotes.Text = "Notes: Specify constraint settings for PAL or NTSC DV output."
                cbFFEXEVideoConstraint.SelectedIndex = 1

            ' FLV1
            Case 2
                FFEXEEnableVideoCBR()
                FFEXEEnableVideoQuality()

            ' GIF
            Case 3

                ' H263
            Case 4

                ' H264
            Case 5
                cbFFEXEH264Mode.SelectedIndex = 0
                cbFFEXEH264Level.SelectedIndex = 0
                cbFFEXEH264Preset.SelectedIndex = 0
                cbFFEXEH264Profile.SelectedIndex = 0
                tbFFEXEH264Quantizer.Value = 23
                cbFFEXEH264QuickTimeCompatibility.Checked = True
                cbFFEXEH264ZeroTolerance.Checked = False
                cbFFEXEH264WebFastStart.Checked = False

            ' HEVC
            Case 6
                cbFFEXEH264Mode.SelectedIndex = 0
                cbFFEXEH264Level.SelectedIndex = 0
                cbFFEXEH264Preset.SelectedIndex = 0
                cbFFEXEH264Profile.SelectedIndex = 0
                tbFFEXEH264Quantizer.Value = 23
                cbFFEXEH264QuickTimeCompatibility.Checked = True
                cbFFEXEH264ZeroTolerance.Checked = False
                cbFFEXEH264WebFastStart.Checked = False


            ' HuffYUV
            Case 7


                ' JPEG 2000
            Case 8


                ' JPEG-LS
            Case 9


                ' LJPEG
            Case 10


                ' MJPEG
            Case 11
                FFEXEEnableVideoQuality()

                tbFFEXEVideoQuality.Value = 4
                tbFFEXEVideoQuality_Scroll(Nothing, Nothing)

            ' MPEG-1
            Case 12
                FFEXEEnableVideoCBR()

                edFFEXEVideoBitrateMin.Text = "1000"
                edFFEXEVideoBitrateMax.Text = "2000"
                edFFEXEVideoTargetBitrate.Text = "1800"

            ' MPEG-2
            Case 13
                FFEXEEnableVideoCBR()

                edFFEXEVideoBitrateMin.Text = "2000"
                edFFEXEVideoBitrateMax.Text = "5000"
                edFFEXEVideoTargetBitrate.Text = "4000"

                edFFEXEVideoGOPSize.Text = "45"
                edFFEXEVideoBFramesCount.Text = "2"

            ' MPEG-4
            Case 14
                FFEXEEnableVideoCBR()

                edFFEXEVideoBitrateMin.Text = "2000"
                edFFEXEVideoBitrateMax.Text = "5000"
                edFFEXEVideoTargetBitrate.Text = "4000"

            ' PNG
            Case 15

                ' Theora
            Case 16
                FFEXEEnableVideoQuality()

                tbFFEXEVideoQuality.Minimum = 0
                tbFFEXEVideoQuality.Maximum = 10
                tbFFEXEVideoQuality.Value = 7
                tbFFEXEVideoQuality_Scroll(Nothing, Nothing)

                edFFEXEVideoTargetBitrate.Text = "2000"

            ' VP8
            Case 17
                FFEXEEnableVideoQuality()
                FFEXEEnableVideoCBR()

                tbFFEXEVideoQuality.Minimum = 4
                tbFFEXEVideoQuality.Maximum = 63
                tbFFEXEVideoQuality.Value = 10
                tbFFEXEVideoQuality_Scroll(Nothing, Nothing)

                edFFEXEVideoTargetBitrate.Text = "2000"

            ' VP9   
            Case 18

                FFEXEEnableVideoQuality()
                FFEXEEnableVideoCBR()

                tbFFEXEVideoQuality.Minimum = 4
                tbFFEXEVideoQuality.Maximum = 63
                tbFFEXEVideoQuality.Value = 10
                tbFFEXEVideoQuality_Scroll(Nothing, Nothing)

                edFFEXEVideoTargetBitrate.Text = "2000"

        End Select

    End Sub

    Private Sub FFEXEDisableAudioMode()

        rbFFEXEAudioModeABR.Enabled = False
        rbFFEXEAudioModeABR.Checked = False
        rbFFEXEAudioModeCBR.Enabled = False
        rbFFEXEAudioModeCBR.Checked = False
        rbFFEXEAudioModeQuality.Enabled = False
        rbFFEXEAudioModeQuality.Checked = False
        rbFFEXEAudioModeLossless.Enabled = False
        rbFFEXEAudioModeLossless.Checked = False

        tbFFEXEAudioQuality.Enabled = False
        cbFFEXEAudioBitrate.Enabled = False

    End Sub

    Private Sub FFEXEEnableAudioCBR()

        rbFFEXEAudioModeCBR.Enabled = True
        rbFFEXEAudioModeCBR.Checked = True

        cbFFEXEAudioBitrate.Enabled = True

    End Sub

    Private Sub FFEXEEnableAudioABR()

        rbFFEXEAudioModeABR.Enabled = True
        rbFFEXEAudioModeABR.Checked = True

        ' edFFEXEAudioTargetBitrate.Enabled = true
        ' edFFEXEAudioBitrateMax.Enabled = true
        ' edFFEXEAudioBitrateMin.Enabled = true

    End Sub

    Private Sub FFEXEEnableAudioQuality()

        rbFFEXEAudioModeQuality.Enabled = True
        rbFFEXEAudioModeQuality.Checked = True

        tbFFEXEAudioQuality.Enabled = True

    End Sub

    Private Sub FFEXEEnableAudioLossless()

        rbFFEXEAudioModeLossless.Enabled = True
        rbFFEXEAudioModeLossless.Checked = True

        ' tbFFEXEAudioQuality.Enabled = true

    End Sub

    Private Sub cbFFEXEAudioCodec_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbFFEXEAudioCodec.SelectedIndexChanged

        FFEXEDisableAudioMode()
        lbFFEXEAudioNotes.Text = "Notes: None."

        Select Case (cbFFEXEAudioCodec.SelectedIndex)
            ' Auto / None
            Case 0


                ' AAC
            Case 1
                FFEXEEnableAudioCBR()

            ' AC3
            Case 2
                FFEXEEnableAudioCBR()

            ' G722
            Case 3

                ' G726
            Case 4

                ' ADPCM
            Case 5

                ' ALAC
            Case 6
                FFEXEEnableAudioCBR()
                FFEXEEnableAudioLossless()

            ' AMR-NB
            Case 7

                ' AMR-WB
            Case 8

                ' E-AC3
            Case 9
                FFEXEEnableAudioCBR()

            ' FLAC
            Case 10

                lbFFEXEAudioNotes.Text = "Notes: Use Quality mode, trackbar set compression level (0-12, ."

                FFEXEEnableAudioQuality()

                tbFFEXEAudioQuality.Minimum = 0
                tbFFEXEAudioQuality.Maximum = 12
                tbFFEXEAudioQuality.Value = 5
                lbFFEXEAudioQuality.Text = tbFFEXEAudioQuality.Value.ToString()

            ' G723
            Case 11

                ' MP2
            Case 12
                FFEXEEnableAudioQuality()
                FFEXEEnableAudioCBR()

                tbFFEXEAudioQuality.Minimum = 0
                tbFFEXEAudioQuality.Maximum = 9
                tbFFEXEAudioQuality.Value = 0
                lbFFEXEAudioQuality.Text = tbFFEXEAudioQuality.Value.ToString()

            ' MP3
            Case 13
                FFEXEEnableAudioQuality()
                FFEXEEnableAudioCBR()

                tbFFEXEAudioQuality.Minimum = 0
                tbFFEXEAudioQuality.Maximum = 9
                tbFFEXEAudioQuality.Value = 4
                lbFFEXEAudioQuality.Text = tbFFEXEAudioQuality.Value.ToString()

            ' OPUS
            Case 14
                FFEXEEnableAudioCBR()

            ' PCM ALAW
            Case 15

                ' PCM F32BE
            Case 16

                ' PCM F32LE
            Case 17

                ' PCM F64BE
            Case 18

                ' PCM F64LE
            Case 19

                ' PCM MULAW
            Case 20

                ' PCM S16BE
            Case 21

                ' PCM S16BE Planar
            Case 22

                ' PCM S16LE
            Case 23

                ' PCM S16LE Planar
            Case 24

                ' PCM S24BE
            Case 25

                ' PCM S24LE
            Case 26

                ' PCM S24LE Planar
            Case 27

                ' PCM S32BE
            Case 28

                ' PCM S32LE
            Case 29

                ' PCM S32LE Planar
            Case 30

                ' PCM S8
            Case 31

                ' PCM S8 Planar
            Case 32

                ' PCM U16BE
            Case 33

                ' PCM U16LE
            Case 34

                ' PCM U24BE
            Case 35

                ' PCM U24LE
            Case 36

                ' PCM U32BE     
            Case 37

                ' PCM U32LE
            Case 38

                ' PCM U8
            Case 39

                ' Speex
            Case 40
                FFEXEEnableAudioQuality()
                FFEXEEnableAudioCBR()

                tbFFEXEAudioQuality.Minimum = 0
                tbFFEXEAudioQuality.Maximum = 10
                tbFFEXEAudioQuality.Value = 5
                tbFFEXEAudioQuality_Scroll(Nothing, Nothing)

            ' Vorbis
            Case 41
                FFEXEEnableAudioQuality()
                FFEXEEnableAudioCBR()

                tbFFEXEAudioQuality.Minimum = -1
                tbFFEXEAudioQuality.Maximum = 10
                tbFFEXEAudioQuality.Value = 5
                tbFFEXEAudioQuality_Scroll(Nothing, Nothing)

            ' WavPack
            Case 42
        End Select

    End Sub

    Private Sub tbFFEXEVideoQuality_Scroll(sender As Object, e As EventArgs) Handles tbFFEXEVideoQuality.Scroll

        lbFFEXEVideoQuality.Text = tbFFEXEVideoQuality.Value.ToString()

    End Sub

    Private Sub tbFFEXEH264Quantizer_Scroll(sender As Object, e As EventArgs) Handles tbFFEXEH264Quantizer.Scroll

        lbFFEXEH264Quantizer.Text = tbFFEXEH264Quantizer.Value.ToString()

    End Sub

    Private Sub tbFFEXEAudioQuality_Scroll(sender As Object, e As EventArgs) Handles tbFFEXEAudioQuality.Scroll

        lbFFEXEAudioQuality.Text = tbFFEXEAudioQuality.Value.ToString()

    End Sub

    Private Sub FillFFMPEGEXESettings(ByRef ffmpegOutput As VFFFMPEGEXEOutput)

        ffmpegOutput.Custom_AdditionalAudioArgs = edFFEXECustomParametersAudio.Text
        ffmpegOutput.Custom_AdditionalVideoArgs = edFFEXECustomParametersVideo.Text

        If (cbFFEXEUseOnlyAdditionalParameters.Checked) Then

            ffmpegOutput.Custom_AllFFMPEGArgs = edFFEXECustomParametersCommon.Text

        Else

            ffmpegOutput.Custom_AdditionalCommonArgs = edFFEXECustomParametersCommon.Text

        End If

        Select Case (cbFFEXEOutputFormat.SelectedIndex)
            Case 0
                ' 3G2
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.Mobile3G2

            Case 1
                ' 3GP
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.Mobile3GP

            Case 2
                ' AC3
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.AC3

            Case 3
                ' ADTS
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.ADTS

            Case 4
                ' AVI
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.AVI

            Case 5
                ' DTS
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.DTS

            Case 6
                ' DTS-HD
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.DTSHD

            Case 7
                ' DVD (VOB)
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.VOB

            Case 8
                ' E-AC3
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.EAC3

            Case 9
                ' F4V
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.F4V

            Case 10
                ' FLAC
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.FLAC

            Case 11
                ' FLV
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.FLV

            Case 12
                ' GIF
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.GIF

            Case 13
                ' H263
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.H263

            Case 14
                ' H264
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.H264

            Case 15
                ' HEVC
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.HEVC

            Case 16
                ' Matroska
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.Matroska

            Case 17
                ' M4V
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.M4V

            Case 18
                ' MJPEG
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MJPEG

            Case 19
                ' MOV
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MOV

            Case 20
                ' MP2
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MP2

            Case 21
                ' MP3
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MP3

            Case 22
                ' MP4
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MP4

            Case 23
                ' MPEG
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MPEG

            Case 24
                ' MPEGTS
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MPEGTS

            Case 25
                ' MXF
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.MXF

            Case 26
                ' OGG
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.OGG

            Case 27
                ' OPUS
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.OPUS

            Case 28
                ' PSP MP4
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.PSP

            Case 29
                ' RAWVideo
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.RAWVideo

            Case 30
                ' SVCD
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.SVCD

            Case 31
                ' SWF
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.SWF

            Case 32
                ' TrueHD
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.TrueHD

            Case 33
                ' VC1
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.VC1

            Case 34
                ' VCD
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.VCD

            Case 35
                ' WAV
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.WAV

            Case 36
                ' WebM
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.WebM

            Case 37
                ' WTV
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.WTV

            Case 38
                ' WV (WavPack)
                ffmpegOutput.OutputMuxer = VFFFMPEGEXEOutputMuxer.WV

        End Select

        Select Case (cbFFEXEVideoCodec.SelectedIndex)

            Case 0
                ' Auto
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.Auto

            Case 1
                ' DV
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.DVVideo

            Case 2
                ' FLV1
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.FLV1

            Case 3
                ' GIF
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.GIF

            Case 4
                ' H263
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.H263

            Case 5
                ' H264
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.H264

            Case 6
                ' HEVC
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.HEVC

            Case 7
                ' HuffYUV
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.HuffYUV

            Case 8
                ' JPEG 2000
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.JPEG2000

            Case 9
                ' JPEG-LS
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.JPEGLS

            Case 10
                ' LJPEG
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.LJPEG

            Case 11
                ' MJPEG
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.MJPEG

            Case 12
                ' MPEG-1
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.MPEG1Video

            Case 13
                ' MPEG-2
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.MPEG2Video

            Case 14
                ' MPEG-4
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.MPEG4

            Case 15
                ' PNG
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.PNG

            Case 16
                ' Theora
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.Theora

            Case 17
                ' VP8
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.VP8

            Case 18
                ' VP9
                ffmpegOutput.Video_Encoder = VFFFMPEGEXEVideoEncoder.VP9

        End Select

        Select Case (cbFFEXEAspectRatio.SelectedIndex)

            Case 0
                ffmpegOutput.Video_AspectRatioW = 0
                ffmpegOutput.Video_AspectRatioH = 1

            Case 1
                ffmpegOutput.Video_AspectRatioW = 1
                ffmpegOutput.Video_AspectRatioH = 1

            Case 2
                ffmpegOutput.Video_AspectRatioW = 4
                ffmpegOutput.Video_AspectRatioH = 3

            Case 3
                ffmpegOutput.Video_AspectRatioW = 16
                ffmpegOutput.Video_AspectRatioH = 9

        End Select

        If (cbFFEXEVideoResolutionOriginal.Checked) Then

            ffmpegOutput.Video_Width = 0
            ffmpegOutput.Video_Height = 0

        Else

            ffmpegOutput.Video_Width = Convert.ToInt32(edFFEXEVideoWidth.Text)
            ffmpegOutput.Video_Height = Convert.ToInt32(edFFEXEVideoHeight.Text)

        End If

        If (rbFFEXEVideoModeCBR.Checked) Then

            ffmpegOutput.Video_Mode = VFFFMPEGEXEVideoMode.CBR

        ElseIf (rbFFEXEVideoModeQuality.Checked) Then

            ffmpegOutput.Video_Mode = VFFFMPEGEXEVideoMode.Quality

        ElseIf (rbFFEXEVideoModeABR.Checked) Then

            ffmpegOutput.Video_Mode = VFFFMPEGEXEVideoMode.ABR

        End If

        ffmpegOutput.Video_Bitrate = Convert.ToInt32(edFFEXEVideoTargetBitrate.Text) * 1000
        ffmpegOutput.Video_MaxBitrate = Convert.ToInt32(edFFEXEVideoBitrateMax.Text) * 1000
        ffmpegOutput.Video_MinBitrate = Convert.ToInt32(edFFEXEVideoBitrateMin.Text) * 1000
        ffmpegOutput.Video_BufferSize = Convert.ToInt32(edFFEXEVBVBufferSize.Text)
        ffmpegOutput.Video_GOPSize = Convert.ToInt32(edFFEXEVideoGOPSize.Text)
        ffmpegOutput.Video_BFrames = Convert.ToInt32(edFFEXEVideoBFramesCount.Text)
        ffmpegOutput.Video_Interlace = cbFFEXEVideoInterlace.Checked
        ffmpegOutput.Video_Letterbox = cbFFEXEVideoResolutionLetterbox.Checked
        ffmpegOutput.Video_Quality = tbFFEXEVideoQuality.Value

        ffmpegOutput.Video_H264_Quantizer = tbFFEXEH264Quantizer.Value
        ffmpegOutput.Video_H264_Mode = cbFFEXEH264Mode.SelectedIndex
        ffmpegOutput.Video_H264_Preset = cbFFEXEH264Preset.SelectedIndex
        ffmpegOutput.Video_H264_Profile = cbFFEXEH264Profile.SelectedIndex
        ffmpegOutput.Video_H264_QuickTime_Compatibility = cbFFEXEH264QuickTimeCompatibility.Checked
        ffmpegOutput.Video_H264_ZeroTolerance = cbFFEXEH264ZeroTolerance.Checked
        ffmpegOutput.Video_H264_WebFastStart = cbFFEXEH264WebFastStart.Checked
        ffmpegOutput.Video_TVSystem = cbFFEXEVideoConstraint.SelectedIndex

        Select Case (cbFFEXEH264Level.SelectedIndex)

            Case 0
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.None

            Case 1
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level1

            Case 2
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level11

            Case 3
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level12

            Case 4
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level13

            Case 5
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level2

            Case 6
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level21

            Case 7
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level22

            Case 8
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level3

            Case 9
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level31

            Case 10
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level32

            Case 11
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level4

            Case 12
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level41

            Case 13
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level42

            Case 14
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level5

            Case 15
                ffmpegOutput.Video_H264_Level = VFFFMPEGEXEH264Level.Level51

        End Select

        Select Case (cbFFEXEAudioCodec.SelectedIndex)

            Case 0
                ' Auto
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.Auto

            Case 1
                ' AAC
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.AAC

            Case 2
                ' AC3
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.AC3

            Case 3
                ' G722
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.adpcm_g722

            Case 4
                ' G726
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.adpcm_g726

            Case 5
                ' ADPCM
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.adpcm_ms

            Case 6
                ' ALAC
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.ALAC

            Case 7
                ' AMR-NB
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.AMR_NB

            Case 8
                ' AMR-WB
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.AMR_WB

            Case 9
                ' E-AC3
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.EAC3

            Case 10
                ' FLAC
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.FLAC

            Case 11
                ' G723
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.G723_1

            Case 12
                ' MP2
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.MP2

            Case 13
                ' MP3
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.MP3

            Case 14
                ' OPUS
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.OPUS

            Case 15
                ' PCM ALAW
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_ALAW

            Case 16
                ' PCM F32BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_F32BE

            Case 17
                ' PCM F32LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_F32LE

            Case 18
                ' PCM F64BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_F64BE

            Case 19
                ' PCM F64LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_F64LE

            Case 20
                ' PCM MULAW
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_MULAW

            Case 21
                ' PCM S16BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S16BE

            Case 22
                ' PCM S16BE Planar
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S16BE_Planar

            Case 23
                ' PCM S16LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S16LE

            Case 24
                ' PCM S16LE Planar
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S16LE_Planar

            Case 25
                ' PCM S24BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S24BE

            Case 26
                ' PCM S24LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S24LE

            Case 27
                ' PCM S24LE Planar
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S24LE_Planar

            Case 28
                ' PCM S32BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S32BE

            Case 29
                ' PCM S32LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S32LE

            Case 30
                ' PCM S32LE Planar
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S32LE_Planar

            Case 31
                ' PCM S8
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S8

            Case 32
                ' PCM S8 Planar
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_S8_Planar

            Case 33
                ' PCM U16BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U16BE

            Case 34
                ' PCM U16LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U16LE

            Case 35
                ' PCM U24BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U24BE

            Case 36
                ' PCM U24LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U24LE

            Case 37
                ' PCM U32BE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U32BE

            Case 38
                ' PCM U32LE
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U32LE

            Case 39
                ' PCM U8
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.PCM_U8

            Case 40
                ' Speex
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.Speex

            Case 41
                ' Vorbis
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.Vorbis

            Case 42
                ' WavPack
                ffmpegOutput.Audio_Encoder = VFFFMPEGEXEAudioEncoder.WavPack

        End Select

        If (cbFFEXEAudioChannels.SelectedIndex = 0) Then

            ffmpegOutput.Audio_Channels = 0

        Else

            ffmpegOutput.Audio_Channels = Convert.ToInt32(cbFFEXEAudioChannels.Text)

        End If

        If (cbFFEXEAudioSampleRate.SelectedIndex = 0) Then

            ffmpegOutput.Audio_SampleRate = 0

        Else

            ffmpegOutput.Audio_SampleRate = Convert.ToInt32(cbFFEXEAudioSampleRate.Text)

        End If


        If (cbFFEXEAudioBitrate.SelectedIndex = 0) Then

            ffmpegOutput.Audio_Bitrate = 0

        Else

            ffmpegOutput.Audio_Bitrate = Convert.ToInt32(cbFFEXEAudioBitrate.Text) * 1000

        End If

        ffmpegOutput.Audio_Quality = tbFFEXEAudioQuality.Value

        If (rbFFEXEAudioModeCBR.Checked) Then

            ffmpegOutput.Audio_Mode = VFFFMPEGEXEAudioMode.CBR

        ElseIf (rbFFEXEAudioModeQuality.Checked) Then

            ffmpegOutput.Audio_Mode = VFFFMPEGEXEAudioMode.Quality

        ElseIf (rbFFEXEAudioModeABR.Checked) Then

            ffmpegOutput.Audio_Mode = VFFFMPEGEXEAudioMode.ABR

        ElseIf (rbFFEXEAudioModeLossless.Checked) Then

            ffmpegOutput.Audio_Mode = VFFFMPEGEXEAudioMode.Lossless

        End If
    End Sub


    Public Delegate Sub FFMPEGInfoDelegate(ByVal message As String)

    Public Sub FFMPEGInfoDelegateMethod(ByVal message As String)

        mmLog.Text += "(NOT ERROR) FFMPEG " + message + Environment.NewLine

    End Sub

    Private Sub VideoCapture1_OnFFMPEGInfo(sender As Object, e As FFMPEGInfoEventArgs) Handles VideoCapture1.OnFFMPEGInfo

        Dim del As FFMPEGInfoDelegate = New FFMPEGInfoDelegate(AddressOf FFMPEGInfoDelegateMethod)
        BeginInvoke(del, e)

    End Sub

    Private Sub btEncryptionOpenFile_Click(sender As Object, e As EventArgs) Handles btEncryptionOpenFile.Click

        If (openFileDialog1.ShowDialog() = DialogResult.OK) Then

            edEncryptionKeyFile.Text = openFileDialog1.FileName

        End If

    End Sub

    Private Sub cbFFEXEH264Mode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbFFEXEH264Mode.SelectedIndexChanged

        FFEXEDisableVideoMode()

        Select Case (cbFFEXEH264Mode.SelectedIndex)
            Case 0
                'CRF
            Case 1
                'CRF (limited bitrate)
                FFEXEEnableVideoCBR()
            Case 2
                'CBR
                FFEXEEnableVideoCBR()
            Case 3
                'ABR
                FFEXEEnableVideoABR()
            Case 4
                'Lossless
        End Select

    End Sub

    Private Sub linkLabel6_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel6.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "http://www.visioforge.com/support/934037-MP4-H264--AAC-output-for-2K--4K-resolution-memory-problem")
        Process.Start(startInfo)

    End Sub

    Private Sub imgTagCover_Click(sender As Object, e As EventArgs) Handles imgTagCover.Click

        If (openFileDialog2.ShowDialog() = DialogResult.OK) Then

            imgTagCover.LoadAsync(openFileDialog2.FileName)
            imgTagCover.Tag = openFileDialog2.FileName

        End If

    End Sub

    Private Sub linkLabel7_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel7.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "https://visioforge-files.s3.amazonaws.com/redists_net/redist_dotnet_vlc_x86.exe")
        Process.Start(startInfo)

    End Sub

    Private Sub FFMPEGDownloadLinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel3.LinkClicked, LinkLabel9.LinkClicked, LinkLabel8.LinkClicked, linkLabel10.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "https://visioforge-files.s3.amazonaws.com/redists_net/redist_dotnet_ffmpeg_exe_x86_x64.exe")
        Process.Start(startInfo)

    End Sub

    Private Sub cbDecklinkCaptureDevice_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbDecklinkCaptureDevice.SelectedIndexChanged

        cbDecklinkCaptureVideoFormat.Items.Clear()

        Dim deviceItem = (From device In VideoCapture1.Decklink_CaptureDevices Where device.Name = cbDecklinkCaptureDevice.Text)?.First()
        If Not IsNothing(deviceItem) Then
            Dim formats = deviceItem.VideoFormats

            For Each format As DecklinkVideoFormat In formats

                cbDecklinkCaptureVideoFormat.Items.Add(format.Name)

            Next

            If (cbDecklinkCaptureVideoFormat.Items.Count = 0) Then

                cbDecklinkCaptureVideoFormat.Items.Add("No input connected")

            End If

            cbDecklinkCaptureVideoFormat.SelectedIndex = 0
            ' cbVideoInputFormat_SelectedIndexChanged(null, null);
        End If

    End Sub

    Private Sub btAudioChannelMapperClear_Click(sender As Object, e As EventArgs) Handles btAudioChannelMapperClear.Click

        audioChannelMapperItems.Clear()
        lbAudioChannelMapperRoutes.Items.Clear()

    End Sub

    Private Sub btAudioChannelMapperAddNewRoute_Click(sender As Object, e As EventArgs) Handles btAudioChannelMapperAddNewRoute.Click

        Dim item As AudioChannelMapperItem = New AudioChannelMapperItem()
        item.SourceChannel = Convert.ToInt32(edAudioChannelMapperSourceChannel.Text)
        item.TargetChannel = Convert.ToInt32(edAudioChannelMapperTargetChannel.Text)
        item.TargetChannelVolume = tbAudioChannelMapperVolume.Value / 1000.0F

        audioChannelMapperItems.Add(item)

        lbAudioChannelMapperRoutes.Items.Add(
                "Source: " + edAudioChannelMapperSourceChannel.Text + ", target: " + edAudioChannelMapperTargetChannel.Text + ", volume: " + (tbAudioChannelMapperVolume.Value / 1000.0F).ToString("F2"))

    End Sub

    Private Sub linkLabel11_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linkLabel11.LinkClicked

        Dim startInfo = New ProcessStartInfo("explorer.exe", "https://visioforge.com/support/577349-Network-streaming-to-YouTube")
        Process.Start(startInfo)

    End Sub

    Public Delegate Sub NetworkStopDelegate()

    Public Sub NetworkStopDelegateMethod()

        VideoCapture1.Stop()

        MessageBox.Show("Network source stopped or disconnected!")

    End Sub

    Private Sub VideoCapture1_OnNetworkSourceDisconnect(sender As Object, e As EventArgs) Handles VideoCapture1.OnNetworkSourceDisconnect

        Dim del As NetworkStopDelegate = New NetworkStopDelegate(AddressOf NetworkStopDelegateMethod)
        BeginInvoke(del, e)

    End Sub

    Private Sub tpNVENC_Enter(sender As Object, e As EventArgs) Handles tpNVENC.Enter

        If (lbNVENCStatus.Tag.ToString() = "0") Then

            lbNVENCStatus.Tag = 1

            Dim errorCode As NVENCErrorCode
            Dim res As Boolean = VideoCapture.Filter_Supported_NVENC(errorCode)

            If (res) Then
                lbNVENCStatus.Text = "Available"
            Else
                lbNVENCStatus.Text = "Not available. Error code: " + errorCode
            End If

        End If

    End Sub

    Private Sub tbGPULightness_Scroll(sender As Object, e As EventArgs) Handles tbGPULightness.Scroll

        Dim intf As IVFGPUVideoEffectBrightness
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Brightness")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectBrightness(True, tbGPULightness.Value)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbGPULightness.Value
                intf.Update()
            End If
        End If

    End Sub

    Private Sub tbGPUSaturation_Scroll(sender As Object, e As EventArgs) Handles tbGPUSaturation.Scroll

        Dim intf As IVFGPUVideoEffectSaturation
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Saturation")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectSaturation(True, tbGPUSaturation.Value)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbGPUSaturation.Value
                intf.Update()
            End If
        End If

    End Sub

    Private Sub tbGPUContrast_Scroll(sender As Object, e As EventArgs) Handles tbGPUContrast.Scroll

        Dim intf As IVFGPUVideoEffectContrast
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Contrast")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectContrast(True, tbGPUContrast.Value)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbGPUContrast.Value
                intf.Update()
            End If
        End If

    End Sub

    Private Sub tbGPUDarkness_Scroll(sender As Object, e As EventArgs) Handles tbGPUDarkness.Scroll

        Dim intf As IVFGPUVideoEffectDarkness
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Darkness")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectDarkness(True, tbGPUDarkness.Value)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Value = tbGPUDarkness.Value
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUGreyscale_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUGreyscale.CheckedChanged

        Dim intf As IVFGPUVideoEffectGrayscale
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Grayscale")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectGrayscale(cbGPUGreyscale.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUGreyscale.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUInvert_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUInvert.CheckedChanged

        Dim intf As IVFGPUVideoEffectInvert
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Invert")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectInvert(cbGPUInvert.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUInvert.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUNightVision_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUNightVision.CheckedChanged

        Dim intf As IVFGPUVideoEffectNightVision
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("NightVision")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectNightVision(cbGPUNightVision.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUNightVision.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUPixelate_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUPixelate.CheckedChanged

        Dim intf As IVFGPUVideoEffectPixelate
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Pixelate")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectPixelate(cbGPUPixelate.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUPixelate.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUDenoise_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUDenoise.CheckedChanged

        Dim intf As IVFGPUVideoEffectDenoise
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Denoise")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectDenoise(cbGPUDenoise.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUDenoise.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUDeinterlace_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUDeinterlace.CheckedChanged

        Dim intf As IVFGPUVideoEffectDeinterlaceBlend
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("DeinterlaceBlend")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectDeinterlaceBlend(cbGPUDeinterlace.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUDeinterlace.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUBlur_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUBlur.CheckedChanged

        Dim intf As IVFGPUVideoEffectBlur
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("Blur")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectBlur(cbGPUBlur.Checked, 50)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUBlur.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub cbGPUOldMovie_CheckedChanged(sender As Object, e As EventArgs) Handles cbGPUOldMovie.CheckedChanged

        Dim intf As IVFGPUVideoEffectOldMovie
        Dim effect = VideoCapture1.Video_Effects_GPU_Get("OldMovie")
        If (IsNothing(effect)) Then
            intf = New VFGPUVideoEffectOldMovie(cbGPUOldMovie.Checked)
            VideoCapture1.Video_Effects_GPU_Add(intf)
        Else
            intf = effect
            If (Not IsNothing(intf)) Then
                intf.Enabled = cbGPUOldMovie.Checked
                intf.Update()
            End If
        End If

    End Sub

    Private Sub TabPage26_Click(sender As Object, e As EventArgs) Handles TabPage26.Click

    End Sub

    Private Sub btShowIPCamDatabase_Click(sender As Object, e As EventArgs) Handles btShowIPCamDatabase.Click

        IPCameraDB.IPCameraDB.ShowWindow()

    End Sub

    Private Sub btONVIFConnect_Click(sender As Object, e As EventArgs) Handles btONVIFConnect.Click

        If (btONVIFConnect.Text = "Connect") Then
            btONVIFConnect.Text = "Disconnect"

            If (onvifControl IsNot Nothing) Then
                onvifControl.Disconnect()
                onvifControl.Dispose()
                onvifControl = Nothing
            End If

            If (String.IsNullOrEmpty(edIPLogin.Text) Or String.IsNullOrEmpty(edIPPassword.Text)) Then
                MessageBox.Show("Please specify IP camera user name and password.")
                Exit Sub
            End If

            onvifControl = New ONVIFControl()
            Dim result = onvifControl.Connect(edIPUrl.Text, edIPLogin.Text, edIPPassword.Text)

            If (Not result) Then
                onvifControl = Nothing
                MessageBox.Show("Unable to connect to ONVIF camera.")
                Exit Sub
            End If

            Dim deviceInfo = onvifControl.GetDeviceInformation()
            lbONVIFCameraInfo.Text = $"Model {deviceInfo.Model}, Firmware {deviceInfo.Firmware}"

            cbONVIFProfile.Items.Clear()

            Dim profiles As VisioForge.MediaFramework.deviceio.Profile() = onvifControl.GetProfiles()
            For Each profile As VisioForge.MediaFramework.deviceio.Profile In profiles
                cbONVIFProfile.Items.Add($"{profile.Name}")
            Next

            If (cbONVIFProfile.Items.Count > 0) Then
                cbONVIFProfile.SelectedIndex = 0
            End If

            edONVIFLiveVideoURL.Text = onvifControl.GetVideoURL()

            onvifPtzRanges = onvifControl.PTZ_GetRanges()
            onvifControl.PTZ_SetAbsolute(0, 0, 0)

            onvifPtzX = 0
            onvifPtzY = 0
            onvifPtzZoom = 0
        Else
            btONVIFConnect.Text = "Connect"

            If (onvifControl IsNot Nothing) Then
                onvifControl.Disconnect()
                onvifControl.Dispose()
                onvifControl = Nothing
            End If
        End If

    End Sub

    Private Sub btONVIFRight_Click(sender As Object, e As EventArgs) Handles btONVIFRight.Click

        If (onvifControl Is Nothing Or onvifPtzRanges Is Nothing) Then
            Exit Sub
        End If

        Dim step_ As Double = (onvifPtzRanges.MaxX - onvifPtzRanges.MinX) / 30
        onvifPtzX = onvifPtzX - step_

        If (onvifPtzX < onvifPtzRanges.MinX) Then
            onvifPtzX = onvifPtzRanges.MinX
        End If

        onvifControl?.PTZ_SetAbsolute(onvifPtzX, onvifPtzY, onvifPtzZoom)

    End Sub

    Private Sub btONVIFPTZSetDefault_Click(sender As Object, e As EventArgs) Handles btONVIFPTZSetDefault.Click

        onvifControl?.PTZ_SetAbsolute(0, 0, 0)

    End Sub

    Private Sub btONVIFLeft_Click(sender As Object, e As EventArgs) Handles btONVIFLeft.Click

        If (onvifControl Is Nothing Or onvifPtzRanges Is Nothing) Then
            Exit Sub
        End If

        Dim step_ As Double = (onvifPtzRanges.MaxX - onvifPtzRanges.MinX) / 30
        onvifPtzX = onvifPtzX + step_

        If (onvifPtzX > onvifPtzRanges.MaxX) Then
            onvifPtzX = onvifPtzRanges.MaxX
        End If

        onvifControl?.PTZ_SetAbsolute(onvifPtzX, onvifPtzY, onvifPtzZoom)

    End Sub

    Private Sub btONVIFUp_Click(sender As Object, e As EventArgs) Handles btONVIFUp.Click

        If (onvifControl Is Nothing Or onvifPtzRanges Is Nothing) Then
            Exit Sub
        End If

        Dim step_ As Double = (onvifPtzRanges.MaxY - onvifPtzRanges.MinY) / 30
        onvifPtzY = onvifPtzY - step_

        If (onvifPtzY < onvifPtzRanges.MinY) Then
            onvifPtzY = onvifPtzRanges.MinY
        End If

        onvifControl?.PTZ_SetAbsolute(onvifPtzX, onvifPtzY, onvifPtzZoom)

    End Sub

    Private Sub btONVIFDown_Click(sender As Object, e As EventArgs) Handles btONVIFDown.Click

        If (onvifControl Is Nothing Or onvifPtzRanges Is Nothing) Then
            Exit Sub
        End If

        Dim step_ As Double = (onvifPtzRanges.MaxY - onvifPtzRanges.MinY) / 30
        onvifPtzY = onvifPtzY + step_

        If (onvifPtzY > onvifPtzRanges.MaxY) Then
            onvifPtzY = onvifPtzRanges.MaxY
        End If

        onvifControl?.PTZ_SetAbsolute(onvifPtzX, onvifPtzY, onvifPtzZoom)

    End Sub

    Private Sub btONVIFZoomIn_Click(sender As Object, e As EventArgs) Handles btONVIFZoomIn.Click

        If (onvifControl Is Nothing Or onvifPtzRanges Is Nothing) Then
            Exit Sub
        End If

        Dim step_ As Double = (onvifPtzRanges.MaxZoom - onvifPtzRanges.MinZoom) / 100
        onvifPtzZoom = onvifPtzZoom + step_

        If (onvifPtzZoom > onvifPtzRanges.MaxZoom) Then
            onvifPtzZoom = onvifPtzRanges.MaxZoom
        End If

        onvifControl?.PTZ_SetAbsolute(onvifPtzX, onvifPtzY, onvifPtzZoom)

    End Sub

    Private Sub btONVIFZoomOut_Click(sender As Object, e As EventArgs) Handles btONVIFZoomOut.Click

        If (onvifControl Is Nothing Or onvifPtzRanges Is Nothing) Then
            Exit Sub
        End If

        Dim step_ As Double = (onvifPtzRanges.MaxZoom - onvifPtzRanges.MinZoom) / 100
        onvifPtzZoom = onvifPtzZoom - step_

        If (onvifPtzZoom < onvifPtzRanges.MinZoom) Then
            onvifPtzZoom = onvifPtzRanges.MinZoom
        End If

        onvifControl?.PTZ_SetAbsolute(onvifPtzX, onvifPtzY, onvifPtzZoom)

    End Sub

    Private Sub pnPIPChromaKeyColor_Click(sender As Object, e As EventArgs) Handles pnPIPChromaKeyColor.Click

        colorDialog1.Color = pnPIPChromaKeyColor.BackColor

        If (colorDialog1.ShowDialog() = DialogResult.OK) Then
            pnPIPChromaKeyColor.BackColor = colorDialog1.Color
        End If

    End Sub

    Private Sub tbPIPChromaKeyTolerance1_Scroll(sender As Object, e As EventArgs) Handles tbPIPChromaKeyTolerance1.Scroll

        lbPIPChromaKeyTolerance1.Text = tbPIPChromaKeyTolerance1.Value.ToString()

    End Sub

    Private Sub tbPIPChromaKeyTolerance2_Scroll(sender As Object, e As EventArgs) Handles tbPIPChromaKeyTolerance2.Scroll

        lbPIPChromaKeyTolerance2.Text = tbPIPChromaKeyTolerance2.Value.ToString()

    End Sub
End Class

' ReSharper restore InconsistentNaming