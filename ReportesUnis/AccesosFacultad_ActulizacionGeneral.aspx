<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AccesosFacultad_ActulizacionGeneral.aspx.cs" Inherits="ReportesUnis.AccesosFacultad_ActulizacionGeneral" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
    <asp:Label ID="TxtURL" runat="server" Visible="false"></asp:Label>
    <div class="container">
        <br />
        <h2 style="text-align: center;">ACCESOS ACTUALIZACION</h2>
    </div>
    <hr />
    <div class="container" id="Div1" runat="server">
        <div class="row justify-content-center">
            <div class="col-md-8">
                <div class="container">
                    <!-- Fila para los Labels -->
                    <div class="row">
                        <div class="form-group col-md-6 text-center">
                            <asp:Label runat="server" Font-Bold="true">Tipo de Búsqueda:</asp:Label>
                            <br />
                            <asp:DropDownList ID="CmbBusqueda" runat="server" Width="275px" CssClass="form-control mx-auto">
                                <asp:ListItem>Documento de Identificación</asp:ListItem>
                                <asp:ListItem>Nombre</asp:ListItem>
                                <asp:ListItem>Facultad</asp:ListItem>
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
                            <asp:Button ID="BtnBuscar" runat="server" Text="Buscar" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnBuscar_Click" />
                            <asp:Button ID="BtnLimpiarBusqueda" runat="server" Text="Limpiar" CssClass="btn-danger-unis" Enabled="false" OnClick="BtnLimpiarBusqueda_Click" />
                            <asp:Button ID="BtnNuevo" runat="server" Text="Nuevo Registro" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnNuevo_Click" />
                            <asp:Button ID="BtnEliminar" runat="server" Text="Eliminar Registro" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnEliminar_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <hr />
    <asp:GridView ID="GridViewInformación" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered centrado-horizontal centrado" OnRowDataBound="GridViewInformación_RowDataBound">
        <Columns>
            <asp:TemplateField HeaderText="Eliminar" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:CheckBox ID="CheckBoxRegistro" runat="server" Font-Size="Large" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="NOMBRE" HeaderText="Nombre" />
            <asp:BoundField DataField="DPI" HeaderText="DPI" />
            <asp:BoundField DataField="COD_FACULTAD" HeaderText="Facultad" />
            <asp:BoundField DataField="FECHA_REGISTRO" HeaderText="Fecha registro" />
        </Columns>
    </asp:GridView>

    <div id="myModalNuevoRegistro" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 style="text-align: center; color: darkred; text-align: center"><strong>Información encontrada</strong></h4>
                    <span class="closeBusqueda">&times;</span>
                </div>

                <div class="modal-body">
                    <contenttemplate>
                        <div class="container emp-profile">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="profile-head">
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:Label ID="Label8" runat="server" Font-Bold="true" ForeColor="Black">Selecciona una opción</asp:Label>
                                                <br />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:Button ID="BtnAgregar" runat="server" Text="Agregar" CssClass="btn-danger-unis" Enabled="true" CausesValidation="false" OnClick="BtnAgregar_Click" />
                                                <br />
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="form-group col-md" style="max-height: calc(100vh - 150px); overflow-y: auto;">
                                                <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                                <div class="containerGV" id="GVContainer">
                                                    <asp:GridView ID="GridViewBusqueda" runat="server" AutoGenerateColumns="false"
                                                        CssClass="table table-condensed table-bordered" OnRowDataBound="GridViewBusqueda_RowDataBound">
                                                        <Columns>
                                                            <asp:TemplateField HeaderText="Seleccionar" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:RadioButton ID="RBBusqueda" runat="server" GroupName="BusquedaGroup" OnClick="selectOnlyThis(this)" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="DPI" HeaderText="DPI" />
                                                            <asp:BoundField DataField="NAME" HeaderText="Nombre" ItemStyle-CssClass="nowrap" />
                                                            <asp:TemplateField HeaderText="Facultad">
                                                                <ItemTemplate>
                                                                    <asp:DropDownList ID="CmbFacultades" runat="server">
                                                                    </asp:DropDownList>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
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

    <div id="myModalNoExiste" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100vh; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog" role="document" style="display: flex; flex-direction: column; align-items: center;">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 style="text-align: center; color: darkred;"><strong>Alerta</strong></h5>
                    <span class="closeNoExiste" style="cursor: pointer;">&times;</span>
                </div>
                <div class="modal-body">
                    <contenttemplate>
                        <div class="container emp-profile">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="profile-head">
                                        <div class="row">
                                            <div class="form-group col-md">
                                                <asp:Label ID="Label7" runat="server" ForeColor="Black">No se encontró información relacionada a la información proporcionada</asp:Label>
                                                <br />
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

    <div id="myModalEliminado" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100vh; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog" role="document" style="display: flex; flex-direction: column; align-items: center;">
            <div class="modal-content">
                <div class="row">
                    <div class="col-md-12 mx-auto text-center">
                        <div style="margin-bottom: 20px;"></div>
                        <div class="modal-messageCarne">Se eliminó correctamente la información</div>
                        <div style="margin-bottom: 20px;"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal" id="myModalError" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="row">
                    <div class="col-md-12 mx-auto text-center">
                        <div style="margin-bottom: 20px;"></div>
                        <div class="modal-messageCarne">Ocurrió un error, intente más tarde.</div>
                        <div style="margin-bottom: 20px;"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal" id="myModalEliminar" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="row">
                    <div class="col-md-12 mx-auto text-center">
                        <div style="margin-bottom: 20px;"></div>
                        <div class="modal-messageCarne">Es necesario seleccionar los registros que desea eliminar.</div>
                        <div style="margin-bottom: 20px;"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal" id="myModalRequerido" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="row">
                    <div class="col-md-12 mx-auto text-center">
                        <div style="margin-bottom: 20px;"></div>
                        <div class="modal-messageCarne">Es necesario seleccionar la información del permiso a otorgar.</div>
                        <div style="margin-bottom: 20px;"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    
    <div id="myModalAgregado" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100vh; justify-content: center; align-items: center; z-index: 9999;">
        <div class="modal-dialog" role="document" style="display: flex; flex-direction: column; align-items: center;">
            <div class="modal-content">
                <div class="row">
                    <div class="col-md-12 mx-auto text-center">
                        <div style="margin-bottom: 20px;"></div>
                        <div class="modal-messageCarne">Se otorgo el permiso correctamente.</div>
                        <div style="margin-bottom: 20px;"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <script>
        function Busqueda() {
            $('#myModalNuevoRegistro').css('display', 'block');
            return false;
        }

        $('.closeBusqueda').click(function () {
            $('#myModalNuevoRegistro').css('display', 'none');
        });


        function NoExiste() {
            $('#myModalNoExiste').css('display', 'block');
            $('#myModalNuevoRegistro').css('display', 'none');
        }

        $('.closeNoExiste').click(function () {
            $('#myModalNoExiste').css('display', 'none');
        });

        function Eliminado() {
            var modal = document.getElementById("myModalEliminado");
            modal.style.display = "block";

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
                window.location.href = "AccesosFacultad_ActulizacionGeneral.aspx";
            }, 4000); // 4000 milisegundos =  segundos
        }

        function mostrarModalError() {
            var modal = document.getElementById("myModalError");
            modal.style.display = "block";

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
                window.location.href = "AccesosFacultad_ActulizacionGeneral.aspx";
            }, 4000); // 4000 milisegundos =  segundos
        }

        function mostrarModalEliminar() {
            var modal = document.getElementById("myModalEliminar");
            modal.style.display = "block";

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
            }, 4000); // 4000 milisegundos =  segundos
        }
        function mostrarRequerido() {
            var modal = document.getElementById("myModalRequerido");
            modal.style.display = "block";

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
            }, 4000); // 4000 milisegundos =  segundos
        }

        function Agregado() {
            var modal = document.getElementById("myModalAgregado");
            modal.style.display = "block";

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
                window.location.href = "AccesosFacultad_ActulizacionGeneral.aspx";
            }, 4000); // 4000 milisegundos =  segundos
        }

        function selectOnlyThis(radioButton) {
            var allRadios = document.querySelectorAll('[id*="RBBusqueda"]');
            allRadios.forEach(function (radio) {
                radio.checked = false;
            });
            radioButton.checked = true;
        }

        function UpdateCheckBoxState(checkBox) {
            var rowIndex = checkBox.parentNode.parentNode.rowIndex;
            var checkBoxState = checkBox.checked;
            var checkBoxStateKey = "CheckBoxState_" + rowIndex;

            // Utilizar Ajax para enviar una solicitud asíncrona al servidor
            $.ajax({
                type: "POST",
                url: "AccesosFacultad_ActulizacionGeneral.aspx/UpdateCheckBoxState",
                data: "{ 'checkBoxStateKey': '" + checkBoxStateKey + "', 'checkBoxState': " + checkBoxState + " }",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    // No hacer nada, solo actualizar el estado del checkbox en el servidor
                }
            });
        }
    </script>
</asp:Content>
