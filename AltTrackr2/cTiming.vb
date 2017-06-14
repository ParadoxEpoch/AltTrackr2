﻿Public Class cTiming

    Public Shared Sub transitionForms(fromForm As Form, toForm As Form)
        Dim opacityStepping As Double = 0.1
        toForm.Opacity = 1
        fromForm.Opacity = 1
        toForm.Show()
        toForm.Location = fromForm.Location
        fromForm.TopMost = True
        pause(20)
        Do Until fromForm.Opacity <= 0
            fromForm.Opacity -= opacityStepping
            pause(1)
        Loop
        fromForm.TopMost = False
        fromForm.Close()
    End Sub

    Declare Function GetTickCount Lib "kernel32" Alias "GetTickCount" () As Long

    Public Shared Sub pause(milliseconds As Long)
        Dim i As Long
        i = GetTickCount + milliseconds
        Do While GetTickCount < i
            Application.DoEvents()
        Loop
    End Sub
End Class
