<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="LogSerbipagos.aspx.cs" Inherits="ReportesUnis.LogSerbipagos" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">


    <br />
    <div style="margin-left: auto; margin-right: auto; text-align: center;">
    <asp:Label Text="Reporte de log SerbiPagos" runat="server" CssClass="h3" />
        </div>
    <br />
    <div class="container">
        <div class="row">
            <div class="form-group col-md-4">
                <asp:Label Text="Desde" for="TextBoxFechaIni" runat="server" />
                <asp:TextBox ID="TextBoxFechaIni" CssClass="form-control" runat="server" TextMode="Date" required></asp:TextBox>
                
            </div>
            <div class="form-group col-md-4">
                <asp:Label Text="Hasta" for="TextBoxFechaFin" runat="server" />
                <asp:TextBox ID="TextBoxFechaFin" CssClass="form-control" runat="server" TextMode="Date" required></asp:TextBox>
               
            </div>

            <div class="form-group col-md-4 mb-3">
                <asp:Label Text="Tipo de registro" for="DropDownListTipo" runat="server" />
                <asp:DropDownList ID="DropDownListTipo" runat="server" CssClass="form-control" required>
                    <asp:ListItem Value="">Seleccione...</asp:ListItem>
                    <asp:ListItem Value="1">Consulta</asp:ListItem>
                    <asp:ListItem Value="2">Pago</asp:ListItem>
                    <asp:ListItem Value="3">Todo</asp:ListItem>
                </asp:DropDownList>
                
            </div>
             <div class="form-group col-md-4 mb-3">
                <asp:Label Text="Identificador personal" for="TextBoxID" runat="server" />
                <asp:TextBox ID="TextBoxID" CssClass="form-control " runat="server"></asp:TextBox>
            </div>
        </div>
        <div class="row">
            <div class="col-md-4 align-self-center">
                <asp:Button ID="ButtonAceptar" runat="server" Text="Cargar" type="submit" CssClass="btn btn-primary align-self-center" OnClick="ButtonAceptar_Click" />
            </div>
        </div>
    </div>
    <br />
    <div class="row">
        <div style="margin-left: auto; margin-right: auto; text-align: center;">
        <rsweb:ReportViewer ID="ReportViewerReporte" runat="server" BackColor="" ClientIDMode="AutoID" HighlightBackgroundColor="" InternalBorderColor="204, 204, 204" InternalBorderStyle="Solid" InternalBorderWidth="1px" LinkActiveColor="" LinkActiveHoverColor="" LinkDisabledColor="" PrimaryButtonBackgroundColor="" PrimaryButtonForegroundColor="" PrimaryButtonHoverBackgroundColor="" PrimaryButtonHoverForegroundColor="" SecondaryButtonBackgroundColor="" SecondaryButtonForegroundColor="" SecondaryButtonHoverBackgroundColor="" SecondaryButtonHoverForegroundColor="" SplitterBackColor="" ToolbarDividerColor="" ToolbarForegroundColor="" ToolbarForegroundDisabledColor="" ToolbarHoverBackgroundColor="" ToolbarHoverForegroundColor="" ToolBarItemBorderColor="" ToolBarItemBorderStyle="Solid" ToolBarItemBorderWidth="1px" ToolBarItemHoverBackColor="" ToolBarItemPressedBorderColor="51, 102, 153" ToolBarItemPressedBorderStyle="Solid" ToolBarItemPressedBorderWidth="1px" ToolBarItemPressedHoverBackColor="153, 187, 226" class="col" Height="800px" Width="1110px">
        </rsweb:ReportViewer>
            </div>
    </div>
</asp:Content>
