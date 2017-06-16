﻿Imports MaterialSkin
Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.ComponentModel

Public Class frmHome
    Dim serverResponse As JObject
    Dim totalHoldings As Decimal = CDec(My.Computer.Registry.GetValue(My.Settings.RegLocation, "TotalHoldings", Nothing))
    Dim coinCodes As String = My.Computer.Registry.GetValue(My.Settings.RegLocation, "AppCoins", Nothing)
    Dim fiatMain As String = My.Computer.Registry.GetValue(My.Settings.RegLocation, "AppMainFiat", Nothing)
    Dim fiatCodes As String = My.Computer.Registry.GetValue(My.Settings.RegLocation, "AppAltFiats", Nothing)

    Private Sub frmHome_Load(sender As Object, e As EventArgs) Handles MyBase.Shown
        Dim SkinManager As MaterialSkinManager = MaterialSkinManager.Instance
        SkinManager.AddFormToManage(Me)
        lblPrice.Font = New Font("Roboto Light", 25)
        lblHoldingsFiat.Font = New Font("Roboto Light", 25)
        lblHoldingsCoin.Font = New Font("Roboto Light", 30)
        lblLoading.Font = New Font("Roboto Light", 20)
        'prgLoading.Parent = PictureBox1
        pnlContent.Hide()
        prgLoading.NumberSpoke = 120
        prgLoading.SpokeThickness = 5
        prgLoading.InnerCircleRadius = 30
        prgLoading.OuterCircleRadius = 35
        prgLoading.RotationSpeed = 20
        prgLoading.Active = True
        prgLoading.Visible = True
        lblLoading.Show()
        GetPrices()
    End Sub

    Private Function ParseJSON(APIURL As String)
        Dim client As WebClient = New WebClient()
        Dim reply As String = client.DownloadString(APIURL)
        Return JObject.Parse(reply)
    End Function

    Private Sub MaterialRaisedButton3_Click(sender As Object, e As EventArgs) Handles MaterialRaisedButton3.Click
        GetPrices()
    End Sub

    Private Sub GetPrices()
        If Not bkgGetPrices.IsBusy Then
            cTiming.WriteDebug("Attempting to fetch latest price data...")
            bkgGetPrices.RunWorkerAsync()
        Else
            cTiming.WriteDebug("Attempted to fetch prices, but I'm already working on it.")
        End If
    End Sub

    Private Sub bkgGetPrices_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bkgGetPrices.DoWork
        serverResponse = ParseJSON("https://min-api.cryptocompare.com/data/price?fsym=" + My.Computer.Registry.GetValue(My.Settings.RegLocation, "AppCoins", Nothing) + "&tsyms=" + My.Computer.Registry.GetValue(My.Settings.RegLocation, "AppAltFiats", Nothing))
    End Sub

    Private Sub bkgGetPrices_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bkgGetPrices.RunWorkerCompleted
        lblAltPrices.Text = String.Empty
        Dim fiatArray() As String = fiatCodes.Split(",")
        For Each fiatCode As String In fiatArray
            lblAltPrices.Text += fiatCode + ": " + CDec(serverResponse.SelectToken(fiatCode)).ToString("n2") + " | "
        Next
        lblAltPrices.Text = lblAltPrices.Text.TrimEnd(" ")
        lblAltPrices.Text = lblAltPrices.Text.TrimEnd("|")
        lblPrice.Text = fiatMain + ": " + CDec(serverResponse.SelectToken(fiatMain)).ToString("n2")
        lblHoldingsCoin.Text = totalHoldings.ToString + " " + coinCodes
        prgLoading.Hide()
        lblLoading.Hide()
        pnlContent.Show()
        'lblHoldingsFiat.Text = "$" + (totalHoldings * CDec(serverResponse.SelectToken("USD"))).ToString("n2") + "/$" + (totalHoldings * CDec(serverResponse.SelectToken("AUD"))).ToString("n2")
    End Sub
End Class