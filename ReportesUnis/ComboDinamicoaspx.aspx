<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ComboDinamicoaspx.aspx.cs" Inherits="ReportesUnis.ComboDinamicoaspx" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.min.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/chosen/1.8.7/chosen.jquery.min.js"></script>

    <div>
        <label for="CmbAlergias">Selecciona alergias:</label>
        <asp:ListBox ID="CmbAlergias" runat="server" CssClass="chosen-select form-control" Multiple="true" SelectionMode="Multiple"></asp:ListBox>
    </div>
    <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />

    <script type="text/javascript">
        $(document).ready(function () {
            $(".chosen-select").chosen({
                placeholder_text_multiple: "Selecciona opciones...",
                width: "100%"
            });
        });
    </script>
</asp:Content>
