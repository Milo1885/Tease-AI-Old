Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Threading.Tasks

Public Class PictureBoxEx
    Inherits PictureBox

    Private _imageStream As MemoryStream

    Private _loaderLock As Object = New Object()

    Private _settingImage As Boolean

    <Category("Behavior"), DefaultValue(False), Description("Determines, if image should be resized when PictureBox changes size.")>
    Public Property RefreshOnResize() As Boolean

    ''' <summary>
    ''' Load an image from the given file path or http URL. 
    ''' The image Is resized To the size Of this PictureBox To save memory.
    ''' </summary>
    ''' <param name="url"></param>
    Public Sub LoadFromUrl(url As String)
        Debug.Print("URL = " & url)
        Dim loaderLock As Object = _loaderLock
        SyncLock loaderLock
            Dim flag2 As Boolean = String.IsNullOrEmpty(url)
            If flag2 Then
                Throw New InvalidOperationException("Empty url is not allowed")
            End If
            Try
                Dim uri As Uri = CalculateUri(url)
                Dim isFile As Boolean = uri.IsFile
                If isFile Then
                    Using fStream As FileStream = File.OpenRead(uri.LocalPath)
                        LoadFromStream(fStream)
                    End Using
                Else
                    Using webClient As WebClient = New WebClient()
                        Using wStream As Stream = webClient.OpenRead(uri)
                            LoadFromStream(wStream)
                        End Using
                    End Using
                End If
            Catch
                Dim flag3 As Boolean = Not DesignMode
                If flag3 Then
                    Throw
                End If
                Image = ErrorImage
            End Try
        End SyncLock
    End Sub

    ''' <summary>
    ''' Load an image from the given file path or http URL. Loading
    ''' is done in a background thread. this method returns immediately.
    ''' The image Is resized to the size of this PictureBox to save memory.
    ''' </summary>
    ''' <param name="url"></param>
    Public Sub LoadFromUrlAsync(url As String)
        Dim flag As Boolean = String.IsNullOrEmpty(url)
        If flag Then
            Throw New InvalidOperationException("Empty url is not allowed")
        End If
        Task.Factory.StartNew(Sub()
                                  LoadFromUrl(url)
                              End Sub)
    End Sub

    Private Function CalculateUri(path As String) As Uri
        Dim result As Uri
        Try
            result = New Uri(path)
        Catch ex_0C As UriFormatException
            path = System.IO.Path.GetFullPath(path)
            result = New Uri(path)
        End Try
        Return result
    End Function

    Private Sub DisposeImageStream()
        Dim flag As Boolean = _imageStream IsNot Nothing
        If flag Then
            _imageStream.Dispose()
            _imageStream = Nothing
        End If
    End Sub

    Private Sub LoadFromStream(Optional stream As Stream = Nothing)
        Dim settingImage As Boolean = Me._settingImage
        If Not settingImage Then
            Dim flag As Boolean = stream IsNot Nothing
            If flag Then
                DisposeImageStream()
                _imageStream = New MemoryStream()
                stream.CopyTo(_imageStream)
            End If
            Dim flag2 As Boolean = _imageStream Is Nothing
            If Not flag2 Then
                Me._settingImage = True
                Dim image As Image = Image.FromStream(_imageStream)
                Dim rectangle As Rectangle = ImageRectangleFromSizeMode(SizeMode, image)
                Dim flag3 As Boolean = image.Width > rectangle.Width OrElse image.Height > rectangle.Height
                If flag3 Then
                    Dim flag4 As Boolean = image.RawFormat.Equals(ImageFormat.Gif)
                    If flag4 Then
                        CreateFromGif(image, rectangle.Width, rectangle.Height)
                    Else
                        MyBase.Image = GetThumbnail(image, rectangle.Width, rectangle.Height)
                    End If
                    image.Dispose()
                Else
                    MyBase.Image = image
                End If

                If Not RefreshOnResize Then
                    DisposeImageStream()
                End If
                _settingImage = False
            End If
        End If
    End Sub

    Private Shared Function DeflateRect(rect As Rectangle, padding As Padding) As Rectangle
        rect.X += padding.Left
        rect.Y += padding.Top
        rect.Width -= padding.Horizontal
        rect.Height -= padding.Vertical
        Return rect
    End Function

    Private Function ImageRectangleFromSizeMode(mode As PictureBoxSizeMode, image As Image) As Rectangle
        Dim result As Rectangle = PictureBoxEx.DeflateRect(MyBase.ClientRectangle, MyBase.Padding)
        Dim flag As Boolean = image IsNot Nothing
        If flag Then
            Select Case mode
                Case PictureBoxSizeMode.Normal, PictureBoxSizeMode.AutoSize
                    result.Size = image.Size
                Case PictureBoxSizeMode.CenterImage
                    result.X += (result.Width - image.Width) / 2
                    result.Y += (result.Height - image.Height) / 2
                    result.Size = image.Size
                Case PictureBoxSizeMode.Zoom
                    Dim size As Size = image.Size
                    Dim num As Single = Math.Min(CSng(MyBase.ClientRectangle.Width) / CSng(size.Width), CSng(MyBase.ClientRectangle.Height) / CSng(size.Height))
                    result.Width = CInt((CSng(size.Width) * num))
                    result.Height = CInt((CSng(size.Height) * num))
                    result.X = (MyBase.ClientRectangle.Width - result.Width) / 2
                    result.Y = (MyBase.ClientRectangle.Height - result.Height) / 2
            End Select
        End If
        Return result
    End Function

    Private Sub CreateFromGif(img As Image, width As Integer, height As Integer)
        Dim encoder As ImageCodecInfo = PictureBoxEx.GetEncoder(ImageFormat.Gif)
        ' Params of the first frame.
        Dim encoderParameters As EncoderParameters = New EncoderParameters(1)
        encoderParameters.Param(0) = New EncoderParameter(Imaging.Encoder.SaveFlag, EncoderValue.MultiFrame)
        ' Params of other frames.
        Dim encoderParametersN As EncoderParameters = New EncoderParameters(1)
        encoderParametersN.Param(0) = New EncoderParameter(Imaging.Encoder.SaveFlag, EncoderValue.FrameDimensionTime)
        ' Params for the finalizing call.
        Dim encoderParametersFlush As EncoderParameters = New EncoderParameters(1)
        encoderParametersFlush.Param(0) = New EncoderParameter(Imaging.Encoder.SaveFlag, EncoderValue.Flush)
        Dim flag As Boolean = True
        Dim bitmap As Bitmap = Nothing
        Dim stream As MemoryStream = New MemoryStream()
        Dim frameDimensionsList As Guid() = img.FrameDimensionsList
        For i As Integer = 0 To frameDimensionsList.Length - 1
            Dim guid As Guid = frameDimensionsList(i)
            Dim num As Integer
            For j As Integer = 0 To img.GetFrameCount(New FrameDimension(guid)) - 1
                Dim flag2 As Boolean = flag
                If flag2 Then
                    bitmap = PictureBoxEx.GetThumbnail(img, width, height)
                    Dim propertyItems As PropertyItem() = img.PropertyItems
                    For k As Integer = 0 To propertyItems.Length - 1
                        Dim propertyItem As PropertyItem = propertyItems(k)
                        bitmap.SetPropertyItem(propertyItem)
                    Next
                    bitmap.Save(stream, encoder, encoderParameters)
                    flag = False
                Else
                    img.SelectActiveFrame(New FrameDimension(guid), j)
                    Dim thumbnail As Bitmap = PictureBoxEx.GetThumbnail(img, width, height)
                    Dim propertyItems2 As PropertyItem() = img.PropertyItems
                    For l As Integer = 0 To propertyItems2.Length - 1
                        Dim propertyItem2 As PropertyItem = propertyItems2(l)
                        thumbnail.SetPropertyItem(propertyItem2)
                    Next
                    bitmap.SaveAdd(thumbnail, encoderParametersN)
                End If
                num = j
            Next
        Next
        Dim flag3 As Boolean = bitmap IsNot Nothing
        If flag3 Then
            bitmap.SaveAdd(encoderParametersFlush)
            MyBase.Image = Image.FromStream(stream)
        End If
    End Sub

    Private Shared Function GetThumbnail(img As Image, width As Integer, height As Integer) As Bitmap
        Dim bitmap As Bitmap = New Bitmap(width, height)
        Using graphics As Graphics = Graphics.FromImage(bitmap)
            graphics.SmoothingMode = SmoothingMode.AntiAlias
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
            graphics.DrawImage(img, 0, 0, width, height)
        End Using
        Return bitmap
    End Function

    Private Shared Function GetEncoder(format As ImageFormat) As ImageCodecInfo
        Dim imageDecoders As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        Return imageDecoders.FirstOrDefault(Function(codec As ImageCodecInfo) codec.FormatID = format.Guid)
    End Function

    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        DisposeImageStream() 'after base so animation has been stopped
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If refreshOnResize Then
            LoadFromStream(Nothing)
        End If
    End Sub

    Protected Overrides Sub OnSizeModeChanged(e As EventArgs)
        MyBase.OnSizeModeChanged(e)
        If refreshOnResize Then
            LoadFromStream(Nothing)
        End If
    End Sub
End Class