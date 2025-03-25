<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BusquedaModal.aspx.cs" Inherits="ReportesUnis.BusquedaModal" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div class="container" id="BusquedaNombre" runat="server">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="container">
                    <!-- Fila para los Labels -->
                    <div class="row">
                        <div class="form-group col-md-6 text-center">
                            <asp:Label runat="server" Font-Bold="true">Tipo de Búsqueda:</asp:Label>
                            <br />
                            <asp:DropDownList ID="CmbBusqueda" runat="server" Width="175px" CssClass="form-control mx-auto">
                                <asp:ListItem>DPI</asp:ListItem>
                                <asp:ListItem>Carnet</asp:ListItem>
                                <asp:ListItem>Nombre</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group col-md-6 text-center">
                            <asp:Label runat="server" Font-Bold="true">Dato a buscar:</asp:Label>
                            <br />
                            <asp:TextBox ID="TxtBusqueda" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control mx-auto"></asp:TextBox>
                        </div>
                    </div>
                    <!-- Fila para Botón de búsqueda -->
                    <div class="row justify-content-center">
                        <div class="form-group col-md-12 text-center">
                            <asp:Button ID="BtnBuscar" runat="server" Text="Buscar" CssClass="btn-danger-unis" Enabled="true" CausesValidation="false" OnClick="BtnBuscar_Click" />
                            <asp:Button ID="BtnLimpiarBusqueda" runat="server" Text="Limpiar" CssClass="btn-danger-unis" Enabled="false" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="myModalBusquedaMultiple" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 style="text-align: center; color: darkred; text-align: center"><strong>Información de Busqueda</strong></h4>
                        <span class="closeBusqueda">&times;</span>
                    </div>
                    <div class="modal-body" style="max-height: calc(100vh - 150px); overflow-y: auto;">
                        <contenttemplate>
                            <div class="container emp-profile">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="profile-head">
                                            <div class="row">
                                                <div class="form-group col-md">
                                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="Black">Selecciona una opción</asp:Label>
                                                    <br />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="form-group col-md">
                                                    <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                                    <div class="containerGV" id="GVContainer">
                                                        <asp:GridView ID="GridViewBusqueda" runat="server" AutoGenerateColumns="false"
                                                            CssClass="table table-condensed table-bordered ">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Seleccionar" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:RadioButton ID="CheckBoxImage" runat="server" Font-Size="Large" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="EMPLID" HeaderText="ID" />
                                                                <asp:BoundField DataField="NAME" HeaderText="Nombre" ItemStyle-CssClass="nowrap" />
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </contenttemplate>
                    </div>
                </div>
            </div>
        </div>


    </div>

    <script>
        function Busqueda() {
            $('#myModalBusquedaMultiple').css('display', 'block');
            return false;
        }

        $('.closeBusqueda').click(function () {
            $('#myModalBusquedaMultiple').css('display', 'none');
        });
    </script>
</asp:Content>
