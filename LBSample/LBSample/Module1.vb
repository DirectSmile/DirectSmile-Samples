Module Module1

    Sub Main()

        Dim serviceUrl As String = "http://yourserver/dsmo/lb.asmx"
        Dim username As String = "your username"
        Dim password As String = "your password"

        ' XML for Order
        Dim xml = <?xml version="1.0" encoding="utf-8"?>
                  <Command>
                      <Parameters>
                          <FandV FieldName="Cache" FieldValue="OFF"/>
                          <FandV FieldName="OUTPUT" FieldValue="PDFPREVIEW"/>
                          <FandV FieldName="JPGCOMPRESSION" FieldValue=""/>
                          <FandV FieldName="CONVERTTO" FieldValue=""/>
                          <FandV FieldName="OutputPagePixWidth" FieldValue=""/>
                          <FandV FieldName="PAGERANGE" FieldValue=""/>
                          <FandV FieldName="IMPPREVIEW" FieldValue=""/>
                          <FandV FieldName="SUPPRESSSETIMAGECROPPING" FieldValue=""/>
                      </Parameters>
                      <StaticFields>
                          <FandV FieldName="01NameInPicture" FieldValue="'my first Document'"/>
                          <FandV FieldName="StartMonth" FieldValue="'1'"/>
                          <FandV FieldName="mYear" FieldValue="'2009'"/>
                          <FandV FieldName="zHemisphere" FieldValue="'North'"/>
                          <FandV FieldName="Claim" FieldValue="'Printing Emotions'"/>
                          <FandV FieldName="Logo" FieldValue="'DirectSmile_4c.eps'"/>
                      </StaticFields>
                  </Command>


        ' Create SOAP Client
        Dim lb As New lb.LBSoapClient("LBSoap", serviceUrl)

        ' Authenticate
        Dim sessionId = lb.Authenticate(username, password, String.Empty)

        ' Returns Error or Session ID
        If sessionId.StartsWith("ERR") Then
            Console.WriteLine("Please check credentials: " & sessionId)
            Console.ReadLine()
            Exit Sub
        End If

        ' Request document
        Dim startDocResult As String = lb.StartDoc(sessionId, "CalendarV3", xml.ToString)

        ' Parse result into XElement
        Dim startDocResultXml As XElement = XElement.Parse(startDocResult)
        Do

            Select Case startDocResultXml.@State
                Case 2
                    ' Error
                    Console.WriteLine(startDocResultXml.@LastErrorMsg)
                    Console.ReadLine()
                    Exit Sub
                Case 3, 4
                    ' Ready or Progress
                    Dim pdfUrl As String = startDocResultXml.@PDFUrl
                    If Not String.IsNullOrEmpty(pdfUrl) AndAlso pdfUrl.EndsWith(".pdf") Then
                        ' PDF generated
                        Console.WriteLine("PDF Url: " & pdfUrl)
                        Console.ReadLine()
                        Exit Sub
                    End If
                Case Else
                    'Progress
                    Console.WriteLine(String.Format("Progress: {0} {1}%", startDocResultXml.@LastProgressMsg, startDocResultXml.@LastProgressPercent))
            End Select

            Threading.Thread.Sleep(1000)

            ' Request status
            startDocResult = lb.GetDocStatus(sessionId, startDocResultXml.@Key)
            If Not String.IsNullOrEmpty(startDocResult) Then
                startDocResultXml = XElement.Parse(startDocResult)
            End If
        Loop
    End Sub

End Module
