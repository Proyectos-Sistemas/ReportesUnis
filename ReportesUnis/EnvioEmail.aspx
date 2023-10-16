
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EnvioEmail.aspx.cs" Inherits="ReportesUnis.EnvioEmail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">PRUEBA ENVIO EMAILS</h2>
    </div>
    <hr />
    <div>
         <asp:Button ID="BtnConfirmar" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnConfirmar_Click" />
    </div>
</asp:Content>