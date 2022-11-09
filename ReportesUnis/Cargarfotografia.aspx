
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cargarfotografia.aspx.cs" Inherits="ReportesUnis.Cargarfotografia"  %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">CARGA DE FOTOGRAFÍAS ESTUDIANTES</h2>
        <hr />
    </div>

    <script type="text/javascript">
        function validateFileSize() {
            var uploadControl = document.getElementById('<%= FileUpload1.ClientID %>');
            var SumaTamañoDeArchivos = 0;
            var fsize = 0;
            console.log(uploadControl.files.length);

            if (uploadControl.files.length > 0) {
                for (var i = 0; i <= uploadControl.files.length - 1; i++) {
                    fsize = uploadControl.files.item(i).size;
                    SumaTamañoDeArchivos = SumaTamañoDeArchivos + fsize;
                }

                console.log(SumaTamañoDeArchivos);
                if (SumaTamañoDeArchivos > 1073741824 /*1GB = 1073741824 Bytes*/) {
                    document.getElementById('dvMsg').style.display = "block";

                    var btn = document.getElementById("<%=btnUpload.ClientID%>");
                    btn.disabled = true;
                    return false;
                }
                else {
                    document.getElementById('dvMsg').style.display = "none";
                    var btn = document.getElementById("<%=btnUpload.ClientID%>");
                    btn.disabled = false;
                    return true;
                }

            }
        }
    </script>

    <script>
        function clearFileInputField(divId) {
            document.getElementById(divId).innerHTML = document.getElementById(tagId).innerHTML;
        }
    </script>

    <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>
        <div class="container">
            <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="True" accept="image/jpeg" onchange="validateFileSize();" />
            <div id="dvMsg" style="background-color: Red; color: White; width: 190px; padding: 3px; display: none;">
                El tamaño máximo permitido es de 1 GB
            </div>
            <asp:Button ID="btnUpload" runat="server" Text="Cargar" OnClick="Upload" OnClientClick="container" CssClass="btn-primary" Enabled="false" />
            <hr />
            <asp:Label ID="lblMensaje" runat="server" Font-Bold="true" ForeColor="Black" Text="" Font-Size="Large"></asp:Label>
            <asp:GridView ID="GridView1" class="table table-bordered table-condensed table-responsive table-hover" runat="server"
                AutoGenerateColumns="false" CssClass="table" Visible="false">
                <Columns>
                    <asp:BoundField DataField="EMPLID" HeaderText="File Name" />
                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDownload" runat="server" Text="Download" OnClick="DownloadFile"
                                CommandArgument='<%# Eval("EMPLID") %>'></asp:LinkButton>
                            <asp:Label ID="lblFilePath" runat="server" Text='<%# Eval("EMPLID") %>' Visible="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>  
</asp:Content>