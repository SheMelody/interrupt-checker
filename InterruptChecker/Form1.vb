Public Class Form1

    Function GetCores(ByVal TargetSet As UInteger) As String
        Dim CoreNumber As UInteger = 0
        Dim str As String = "(invalid assignment)"
        Dim ds = False
        Dim TS As UInteger = TargetSet
        While TS > 0
            If TS Mod 2 = 1 Then
                If Not ds Then
                    str = "core" & CoreNumber
                    ds = True
                Else
                    str &= ", core" & CoreNumber
                End If
            End If
            TS = TS \ 2
            CoreNumber = CoreNumber + 1
        End While
        Return str
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim enum_key = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Enum")
        For Each category In enum_key.GetSubKeyNames()
            Dim category_key = enum_key.OpenSubKey(category)
            For Each device In category_key.GetSubKeyNames()
                Dim device_key = category_key.OpenSubKey(device)
                For Each instance In device_key.GetSubKeyNames()
                    Dim instance_key = device_key.OpenSubKey(instance)
                    If Not instance_key.GetSubKeyNames().Contains("Device Parameters") Then
                        Continue For
                    End If
                    Dim parameters_key = instance_key.OpenSubKey("Device Parameters")
                    If Not parameters_key.GetSubKeyNames().Contains("Interrupt Management") Then
                        Continue For
                    End If
                    Dim interrupt_management_key = parameters_key.OpenSubKey("Interrupt Management")
                    If Not interrupt_management_key.GetSubKeyNames().Contains("Affinity Policy - Temporal") Then
                        Continue For
                    End If
                    Dim temporal_key = interrupt_management_key.OpenSubKey("Affinity Policy - Temporal")
                    If Not temporal_key.GetValueNames().Contains("TargetSet") Then
                        Continue For
                    End If
                    Dim TargetSet As UInteger = temporal_key.GetValue("TargetSet")
                    Dim DeviceDesc As String = instance_key.GetValue("DeviceDesc")
                    If DeviceDesc.Contains(";") Then
                        DeviceDesc = DeviceDesc.Split(";")(1)
                    End If
                    Dim Cores = GetCores(TargetSet)
                    Dim item = New ListViewItem({DeviceDesc, Cores})
                    ListView1.Items.Add(item)

                Next
            Next
        Next
        ListView1.Columns(0).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
        ListView1.Columns(1).AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent)
    End Sub
End Class
