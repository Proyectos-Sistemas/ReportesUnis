﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConfirmacionDeFotografias.aspx.cs" Inherits="ReportesUnis.ConfirmacionDeFotografias" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <br />
        <h2 style="text-align: center;">CONFIRMACIÓN DE FOTOGRAFÍAS</h2>
    </div>

    <br />
    <div id="CamposAuxiliares" runat="server" visible="false">
        <%-- TXTPath ALMACENA EL PATH DONDE SE ALMACENARA LA IMAGEN --%>
        <asp:Label ID="txtPathAC" runat="server" Visible="false"></asp:Label>
        <asp:Label ID="txtPathPC" runat="server" Visible="false"></asp:Label>
        <asp:Label ID="txtPathRC" runat="server" Visible="false"></asp:Label>
        <asp:Label ID="txtPath2" runat="server" Visible="false"></asp:Label>
        <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>
        <%-- TXTURLSQL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
        <asp:Label ID="TxtURLSql" runat="server" Visible="false"></asp:Label>
        <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
        <asp:TextBox ID="txtInsertBI" runat="server" Visible="false"></asp:TextBox>
        <%-- txtInsertApexI ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
        <asp:TextBox ID="txtInsertApex" runat="server" Visible="false"></asp:TextBox>
        <asp:TextBox ID="prueba" runat="server" Visible="false"></asp:TextBox>
        <%-- TEXTBOX ALMACENA CONTROL DE TABS--%>
        <input type="hidden" id="ControlTabs" runat="server" />
        <input type="hidden" id="carne" runat="server" />
        <input type="hidden" id="DPI" runat="server" />
    </div>

    <!-- Código ASP.NET para las pestañas -->
    <div class="tab">
        <asp:Button Text="Actualización" ID="Tab1" CssClass="tablinks" runat="server" OnClick="Tab1_Click" />
        <asp:Button Text="Primer Carne" ID="Tab2" CssClass="tablinks" runat="server" OnClick="Tab2_Click" />
        <asp:Button Text="Renovación de Carne" ID="Tab3" CssClass="tablinks" runat="server" OnClick="Tab3_Click" />
    </div>

    <asp:MultiView ID="MainView" runat="server">
        <asp:View ID="View1" runat="server">
            <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                <tr>
                    <td>
                        <div class="container" style="text-align: center">
                            <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
                            </asp:Label>
                        </div>
                        <div class="container-fluid">
                            <div class="row">
                                <div class="col-md-12">
                                    <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                    <div class="containerGV" id="GVContainer">
                                        <asp:GridView ID="GridViewFotos" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered centrado-horizontal centrado" OnRowDataBound="GridViewFotos_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Eliminar" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="CheckBoxImage" runat="server" Font-Size="Large" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="NombreImagen" HeaderText="Nombre de la imagen" />
                                                <asp:TemplateField HeaderText="Imagen" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Image ID="Image1" runat="server" Width="250" Height="250" ImageAlign="Middle" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="container" runat="server">
                            <asp:Table ID="TbEliminarD" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
                                <asp:TableRow>
                                    <%-- ESPACIO 1--%>
                                    <asp:TableCell>
                        <br />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <%-- ESPACIO 2.1--%>
                                    <asp:TableCell>
                                        <asp:Button ID="BtnEliminar" runat="server" Text="Rechazar Seleccionados" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaEliminar()" OnClick="ButtonSubmit_Click" />
                                    </asp:TableCell>
                                    <%-- ESPACIO 2.2--%>
                                    <asp:TableCell>
                                        <asp:Button ID="BtnConfirmar" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaConfirmar()" OnClick="BtnConfirmar_Click" />
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <%-- ESPACIO 3--%>
                                    <asp:TableCell HorizontalAlign="Center">
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </div>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="View2" runat="server">
            <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                <tr>
                    <td>
                        <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                            <tr>
                                <td>
                                    <div class="container" style="text-align: center">
                                        <asp:Label ID="lblActualizacionPC" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
                                        </asp:Label>
                                    </div>
                                    <div class="container-fluid">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                                <div class="containerGV" id="GVContainerPC">
                                                    <asp:GridView ID="GridViewFotosPC" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered centrado-horizontal centrado" OnRowDataBound="GridViewFotosPC_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Eliminar" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="CheckBoxImagePC" runat="server" Font-Size="Large" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="NombreImagen" HeaderText="Nombre de la imagen" />
                                                            <asp:TemplateField HeaderText="Imagen" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Image ID="Image1" runat="server" Width="250" Height="250" ImageAlign="Middle" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <br />
                                    <div class="container" runat="server">
                                        <asp:Table ID="TbEliminarPC" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
                                            <asp:TableRow>
                                                <%-- ESPACIO 1--%>
                                                <asp:TableCell>
                                                 <br />
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <%-- ESPACIO 2.1--%>
                                                <asp:TableCell>
                                                    <asp:Button ID="BtnEliminarPC" runat="server" Text="Rechazar Seleccionados" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaEliminarPC()" OnClick="ButtonSubmitPC_Click" />
                                                </asp:TableCell>
                                                <%-- ESPACIO 2.2--%>
                                                <asp:TableCell>
                                                    <asp:Button ID="BtnConfirmarPC" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaConfirmarPC()" OnClick="BtnConfirmarPC_Click" />
                                                </asp:TableCell>
                                            </asp:TableRow>
                                            <asp:TableRow>
                                                <%-- ESPACIO 3--%>
                                                <asp:TableCell HorizontalAlign="Center">
                                                </asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:View>
        <asp:View ID="View3" runat="server">
            <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                <tr>
                    <td>
                        <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                            <tr>
                                <td>
                                    <table style="width: 100%; border-width: 1px; border-color: #ddd;">
                                        <tr>
                                            <td>
                                                <div class="container" style="text-align: center">
                                                    <asp:Label ID="lblActualizacionRC" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
                                                    </asp:Label>
                                                </div>
                                                <div class="container-fluid">
                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                                            <div class="containerGV" id="GVContainerRC">
                                                                <asp:GridView ID="GridViewFotosRC" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered centrado-horizontal centrado" OnRowDataBound="GridViewFotosRC_RowDataBound">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Eliminar" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:CheckBox ID="CheckBoxImageRC" runat="server" Font-Size="Large" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:BoundField DataField="NombreImagen" HeaderText="Nombre de la imagen" />
                                                                        <asp:TemplateField HeaderText="Imagen" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Image ID="Image1" runat="server" Width="250" Height="250" ImageAlign="Middle" />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <br />
                                                <div class="container" runat="server">
                                                    <asp:Table ID="TbEliminarRC" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 1--%>
                                                            <asp:TableCell>
                                                            <br />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 2.1--%>
                                                            <asp:TableCell>
                                                                <asp:Button ID="BtnEliminarRC" runat="server" Text="Rechazar Seleccionados" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaEliminarRC()" OnClick="ButtonSubmitRC_Click" />
                                                            </asp:TableCell>
                                                            <%-- ESPACIO 2.2--%>
                                                            <asp:TableCell>
                                                                <asp:Button ID="BtnConfirmarRC" runat="server" Text="Confirmar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlertaConfirmarRC()" OnClick="BtnConfirmarRC_Click" />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <%-- ESPACIO 3--%>
                                                            <asp:TableCell HorizontalAlign="Center">
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:View>
    </asp:MultiView>
    <script>
        function mostrarAlertaEliminar() {
            if (confirm("¿Está seguro de desea eliminar las fotografías seleccionadas?")) {
                __doPostBack('<%= BtnEliminar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }
        function mostrarAlertaConfirmar() {
            if (confirm("¿Está seguro de desea confirmar la información?")) {
                __doPostBack('<%= BtnConfirmar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function mostrarAlertaEliminarPC() {
            if (confirm("¿Está seguro de desea eliminar las fotografías seleccionadas?")) {
                __doPostBack('<%= BtnEliminarPC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }
        function mostrarAlertaConfirmarPC() {
            if (confirm("¿Está seguro de desea confirmar la información?")) {
                __doPostBack('<%= BtnConfirmarPC.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }
    </script>
</asp:Content>

