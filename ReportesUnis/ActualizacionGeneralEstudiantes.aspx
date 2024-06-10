<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizacionGeneralEstudiantes.aspx.cs" Inherits="ReportesUnis.ActualizacionGeneralEstudiantes" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div class="container">
        <div class="row">
            <div class="form-group col">
                <h2 style="text-align: center;">ACTUALIZACIÓN DE INFORMACIÓN ESTUDIANTES</h2>
            </div>
        </div>
        <%--<div id="divActividad" runat="server" style="display: block">
            <div class="row">
                <div class="form-group col">
                    <h6 style="text-align: center;">¿Qué desea realizar el día de hoy?*</h6>
                </div>
            </div>
            <div class="row">
                <div class="col-md-2 mx-auto text-center">
                </div>
                <div class="col-md-4 mx-auto text-center">
                    <asp:RadioButton ID="RadioButtonCarne" runat="server" GroupName="Accion" Text="Solicitar carné y actualizar información" />
                </div>
                <div class="col-md-4 mx-auto text-center">
                    <asp:RadioButton ID="RadioButtonActualiza" runat="server" GroupName="Accion" Text="Solo actualizar información" />
                </div>
                <div class="col-md-2 mx-auto text-center">
                </div>
            </div>
        </div>--%>

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
                                <asp:Button ID="BtnBuscar" runat="server" Text="Buscar" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnBuscar_Click" CausesValidation="false" />
                                <asp:Button ID="BtnLimpiarBusqueda" runat="server" Text="Limpiar" CssClass="btn-danger-unis" Enabled="false" OnClick="BtnLimpiarBusqueda_Click" CausesValidation="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="Informacion" runat="server">

            <hr />        
            <div id="CamposAuxiliares" runat="server" visible="false">
                <%-- TEXTBOX USEREMPLID ALMACENA EL EMPLID DEL USUARIO QUE ESTA HACIENDO LA ACTUALIZACION --%>
                <asp:Label ID="UserEmplid" runat="server" Visible="false"></asp:Label>
                <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO--%>
                <asp:TextBox ID="State" runat="server" Visible="true"></asp:TextBox>
                <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO DEL NIT--%>
                <asp:TextBox ID="StateNIT" runat="server" Visible="true"></asp:TextBox>
                <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE TELEFONO O NO--%>
                <asp:Label ID="TruePhone" runat="server" Visible="false"></asp:Label>
                <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE EMAIL O NO--%>
                <asp:Label ID="TrueEmail" runat="server" Visible="false"></asp:Label>
                <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE DIRECCION O NO--%>
                <asp:Label ID="TrueDir" runat="server" Visible="false"></asp:Label>
                <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
                <asp:Label ID="TxtURL" runat="server" Visible="false"></asp:Label>
                <%-- TXTURLSQL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
                <asp:Label ID="TxtURLSql" runat="server" Visible="false"></asp:Label>
                <%-- TXTUSER ALMACENA EL DPI DEL USUARIO QUE ESTA REALIZANDO CAMBIOS --%>
                <asp:Label ID="TextUser" runat="server" Visible="false"></asp:Label>
                <%-- TXTCONTADOR para contar las excepciones encontradas de apellidos --%>
                <asp:TextBox ID="txtContaador" runat="server" Visible="false"></asp:TextBox>
                <%-- TXTPRIMERAPELLIDO  se almacena el primer apellido del estudiante --%>
                <asp:Label ID="txtPrimerApellido" runat="server" Visible="false"></asp:Label>
                <%-- TXTACCION se almacena el primer apellido del estudiante --%>
                <asp:Label ID="txtAccion" runat="server" Visible="false"></asp:Label>
                <%-- TXTTipoACCION se almacena el primer apellido del estudiante --%>
                <asp:Label ID="txtTipoAccion" runat="server" Visible="false"></asp:Label>
                <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT ESPEJO --%>
                <asp:TextBox ID="txtInsert" runat="server" Visible="false"></asp:TextBox>
                <asp:TextBox ID="txtInsertBit" runat="server" Visible="false"></asp:TextBox>
                <asp:TextBox ID="txtControlBit" runat="server" Visible="false"></asp:TextBox>
                <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
                <asp:TextBox ID="txtInsertBI" runat="server" Visible="false"></asp:TextBox>
                <%-- TXTINSERT ALMACENA EL QUERY PARA HACER CONTROL NOMBRES RECIBO --%>
                <asp:TextBox ID="txtUpdateAR" runat="server" Visible="false"></asp:TextBox>
                <asp:TextBox ID="txtUpdateNR" runat="server" Visible="false"></asp:TextBox>
                <asp:TextBox ID="txtControlNR" runat="server" Visible="false"></asp:TextBox>
                <asp:TextBox ID="txtControlAR" runat="server" Visible="false"></asp:TextBox>
                <%-- TXTEXISTE ALMACENA vALORES PARA HACER VALIDACIONES --%>
                <asp:Label ID="txtExiste" runat="server" Visible="false"></asp:Label>
                <%-- TXTEXISTE2 ALMACENA vALORES PARA HACER VALIDACIONES --%>
                <asp:Label ID="txtExiste2" runat="server" Visible="false"></asp:Label>
                <%-- TXTEXISTE2 ALMACENA vALORES PARA HACER VALIDACIONES --%>
                <asp:Label ID="txtExiste4" runat="server" Visible="false"></asp:Label>
                <%-- TXTEXISTE3 ALMACENA vALORES PARA HACER VALIDACIONES --%>
                <asp:TextBox ID="txtExiste3" runat="server" Visible="false"></asp:TextBox>
                <%-- TXTMUNICIPIODPI ALMACENA EL MUNICIPIO DEL DPI --%>
                <asp:Label ID="txtMunicipioDPI" runat="server" Visible="false"></asp:Label>
                <%-- TXTDEPARTAMENTODPI ALMACENA EL QUERY PARA HACER INSERT ESPEJO --%>
                <asp:Label ID="txtDepartamentoDPI" runat="server" Visible="false"></asp:Label>
                <%-- TXTPath ALMACENA EL PATH DONDE SE ALMACENARA LA IMAGEN --%>
                <asp:Label ID="txtPath" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="txtPathAC" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="txtPathPC" runat="server" Visible="false"></asp:Label>
                <asp:Label ID="txtPathRC" runat="server" Visible="false"></asp:Label>
                <%-- TXTPath URL APEX SERVICIO --%>
                <asp:Label ID="txtApex" runat="server" Visible="false"></asp:Label>
                <%-- APELLIDO PARA APEX --%>
                <asp:TextBox ID="txtApellidoAPEX" runat="server" Visible="false"></asp:TextBox>
                <%-- NOMBRE PARA APEX --%>
                <asp:TextBox ID="txtNombreAPEX" runat="server" Visible="false"></asp:TextBox>
                <%-- CONFIRMACION OPERADOR --%>
                <asp:Label ID="txtConfirmacion" runat="server" Visible="false"></asp:Label>
                <%-- ¡tiene pasaporte? --%>
                <asp:Label ID="txtPaisPasaporte" runat="server" Visible="false"></asp:Label>
                <%-- ¡tiene pasaporte? --%>
                <asp:Label ID="txtCantidadImagenesDpi" runat="server" Visible="false">0</asp:Label>
                <asp:TextBox ID="txtDPI" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control" Visible="false"></asp:TextBox>
            </div>
            <div id="InfePersonal" runat="server">

                <div class="container">
                    <div class="row">
                        <div class="col-md-4 mx-auto text-center">
                        </div>
                        <div class="col-md-4 mx-auto text-center">
                            <h5 style="text-align: center;">Información Personal</h5>
                        </div>
                        <div class="col-md-4 mx-auto text-center">
                        </div>
                    </div>
                </div>
                <br />

                <%-- EMPLID A BUSCAR--%>
                <input type="hidden" id="txtEmplid" runat="server" />
                <%-- NOMBRE INICIAL--%>
                <input type="hidden" id="txtNInicial" runat="server" />
                <%-- APELLIDO INICIAL --%>
                <input type="hidden" id="txtAInicial" runat="server" />
                <%-- APELLIDO CASADA INICIAL --%>
                <input type="hidden" id="txtCInicial" runat="server" />
                <%-- CONTROL DE VALIDACION DE NIT--%>
                <input type="hidden" id="ValidacionNit" runat="server" />
                <%-- TEXTBOX ALMACENA SI EL USUARIO TIENE NIT O NO--%>
                <input type="hidden" id="TrueNit" runat="server" />
                <%-- TEXTBOX ALMACENA EL ESTADO CIVIL INICIAL--%>
                <input type="hidden" id="TrueEstadoCivil" runat="server" />
                <%-- TEXTBOX ALMACENA EL EFFDT DE ESTADO CIVIL--%>
                <input type="hidden" id="EFFDT_EC" runat="server" />
                <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION --%>
                <input type="hidden" id="EFFDT_A" runat="server" />
                <%-- TEXTBOX ALMACENA EL EFFDT DE LA DIRECCION NIT--%>
                <input type="hidden" id="EFFDT_A_NIT" runat="server" />
                <%-- TEXTBOX ALMACENA EL EFFDT DEL NOMBRE EL NIT--%>
                <input type="hidden" id="EFFDT_NameR" runat="server" />
                <%-- TEXTBOX ALMACENA EL EFFDT DEL TELEFONO--%>
                <input type="hidden" id="EFFDT_P" runat="server" />
                <%-- TEXTBOX ALMACENA EL FT STUDENT DEL ESTADO CIVIL--%>
                <input type="hidden" id="FT_STUDENT" runat="server" />
                <%-- TEXTBOX ALMACENA EL NIVEL EDUCATIVO DEL ESTADO CIVIL--%>
                <input type="hidden" id="HIGH_LVL" runat="server" />
                <%-- TEXTBOX ALMACENA EL SEXO DEL ESTADO CIVIL--%>
                <input type="hidden" id="SEX_EC" runat="server" />

                <%-- TEXTBOX ALMACENA UP ESTADO CIVIL--%>
                <input type="hidden" id="UP_PERS_DATA_EFFDT" runat="server" />
                <%-- TEXTBOX ALMACENA UP NOMBRE NIT--%>
                <input type="hidden" id="UP_NAMES_NIT" runat="server" />
                <%-- TEXTBOX ALMACENA UP DIRECCION NIT--%>
                <input type="hidden" id="UP_ADDRESSES_NIT" runat="server" />
                <%-- TEXTBOX ALMACENA UP DIRECCION--%>
                <input type="hidden" id="UP_ADDRESSES" runat="server" />
                <%-- TEXTBOX ALMACENA UP TELEFONO--%>
                <input type="hidden" id="UP_PERSONAL_PHONE" runat="server" />
                <%-- TEXTBOX ALMACENA UP CORREO PERSONAL--%>
                <input type="hidden" id="UP_EMAIL_ADDRESSES" runat="server" />

                <%-- TEXTBOX ALMACENA UP BIRTHDATE--%>
                <input type="hidden" id="UP_BIRTHDATE" runat="server" />
                <%-- TEXTBOX ALMACENA UP BIRTHSTATE--%>
                <input type="hidden" id="UP_BIRTHSTATE" runat="server" />
                <%-- TEXTBOX ALMACENA UP BIRTHPLACE--%>
                <input type="hidden" id="UP_BIRTHPLACE" runat="server" />
                <%-- TEXTBOX ALMACENA UP BIRTHCOUNTRY--%>
                <input type="hidden" id="UP_BIRTHCOUNTRY" runat="server" />


                <%-- TEXTBOX ALMACENA UD ESTADO CIVIL--%>
                <input type="hidden" id="UD_PERS_DATA_EFFDT" runat="server" />
                <%-- TEXTBOX ALMACENA UD NOMBRE NIT--%>
                <input type="hidden" id="UD_NAMES_NIT" runat="server" />
                <%-- TEXTBOX ALMACENA UD DIRECCION NIT--%>
                <input type="hidden" id="UD_ADDRESSES_NIT" runat="server" />
                <%-- TEXTBOX ALMACENA UD DIRECCION--%>
                <input type="hidden" id="UD_ADDRESSES" runat="server" />
                <%-- TEXTBOX ALMACENA UD TELEFONO--%>
                <input type="hidden" id="UD_PERSONAL_PHONE" runat="server" />
                <%-- TEXTBOX ALMACENA UD CORREO PERSONAL--%>
                <input type="hidden" id="UD_EMAIL_ADDRESSES" runat="server" />

                <%-- TEXTBOX ALMACENA UD BIRTHDATE--%>
                <input type="hidden" id="UD_BIRTHDATE" runat="server" />
                <%-- TEXTBOX ALMACENA UD BIRTHSTATE--%>
                <input type="hidden" id="UD_BIRTHSTATE" runat="server" />
                <%-- TEXTBOX ALMACENA UD BIRTHPLACE--%>
                <input type="hidden" id="UD_BIRTHPLACE" runat="server" />
                <%-- TEXTBOX ALMACENA UD BIRTHCOUNTRY--%>
                <input type="hidden" id="UD_BIRTHCOUNTRY" runat="server" />

                <%-- TEXTBOX ALMACENA UP DOCUMENTOS DE IDENTIFICACION--%>
                <input type="hidden" id="UP_IDENTIFICACION" runat="server" />
                <%-- TEXTBOX ALMACENA UD DOCUMENTOS DE IDENTIFICACION--%>
                <input type="hidden" id="UD_IDENTIFICACION" runat="server" />


                <%-- CONTROL DE VERSIONES UP Y UD--%>
                <input type="hidden" id="VersionUP" runat="server" />
                <input type="hidden" id="VersionUD" runat="server" />

                <%-- TEXTBOX ALMACENA LA VARIABLE DE SESION--%>
                <input type="text" id="ISESSION" style="display: none" value="0" runat="server" />
                <input type="hidden" id="banderaSESSION" runat="server" />

                <%-- CREDENCIALES NIT--%>
                <input type="hidden" id="CREDENCIALES_NIT" runat="server" />
                <input type="hidden" id="URL_NIT" runat="server" />

                <%-- CONTROL CAMBIO NOMBRES NIT CF--%>
                <input type="hidden" id="InicialNR1" runat="server" />
                <input type="hidden" id="InicialNR2" runat="server" />
                <input type="hidden" id="InicialNR3" runat="server" />
                <input type="hidden" id="ControlCF" runat="server" />
                <input type="hidden" id="ControlCF2" runat="server" />

                <%-- CONTROL PARA ACTUALIZAR O SOLICITAR CARNE--%>
                <input type="hidden" id="ControlAct" runat="server" />

                <%-- VALIDA BUSQUEDA--%>
                <input type="hidden" id="ExisteBusqueda" runat="server" />

                <%-- VALIDA DATOS NACIMIENTO--%>
                <input type="hidden" id="PaisNacimiento" runat="server" />
                <input type="hidden" id="LugarNacimiento" runat="server" />
                <input type="hidden" id="StateNacimiento" runat="server" />

                <%-- VALIDA DOCUMENTO IDENTIFICACION--%>
                <input type="hidden" id="ExistePasaporte" runat="server" />
                <input type="hidden" id="ExisteDPI" runat="server" />


                <%-- TABLA EN LA QUE SE COLOCAN LOS OBJETOS --%>
                <div class="container" id="tabla" runat="server">
                    <div class="row">
                        <div class="col-md">
                            <div class="container">
                                <div class="row">
                                    <div class="form-group col-md 12">
                                        <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                        <div class="containerGV">
                                            <asp:GridView ID="GridViewDocumentos" runat="server" AutoGenerateColumns="false"
                                                CssClass="table table-condensed table-bordered" OnRowDataBound="GridViewDocumentos_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Principal" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:RadioButton ID="RBDocPrincipal" runat="server" GroupName="DocumentosIdentificacion" Checked='<%# Eval("PRIMARY_NID").ToString() == "Y" %>' OnClick="selectOnlyThisDoc(this)" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="País">
                                                        <ItemTemplate>
                                                            <asp:DropDownList ID="DDLPais" runat="server">
                                                            </asp:DropDownList>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="TipoDocumento" HeaderText="Tipo de Documento de Identidad" />
                                                    <asp:TemplateField HeaderText="Documento" ItemStyle-CssClass="nowrap">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="TxtNroDocumento" runat="server" Text='<%# Eval("Documento") %>'></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="PRIMARY_NID" HeaderText="PRIMARY_NID" Visible="false" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Carné:</asp:Label>
                                        <br />
                                        <asp:Label ID="txtCarne" runat="server" Enabled="false"></asp:Label>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Facultad:</asp:Label>
                                        <br />
                                        <asp:Label ID="txtFacultad" runat="server" Enabled="false"></asp:Label>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Carrera:</asp:Label>
                                        <br />
                                        <asp:Label ID="txtCarrera" runat="server" Enabled="false"></asp:Label>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Correo Institucional:</asp:Label>
                                        <br />
                                        <asp:Label ID="EmailUnis" runat="server" Enabled="false"></asp:Label>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Fecha de nacimiento:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtCumple" runat="server" Enabled="true" Width="275px" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Lugar de Nacimiento:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="TxtLugarNac" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                    </div>


                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">País de Nacimiento*:</asp:Label><br />
                                        <asp:DropDownList ID="CmbPaisNacimiento" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbPaisNac_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                        </asp:DropDownList>
                                        <br />
                                    </div>


                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Departamento de Nacimiento*:</asp:Label>
                                        <asp:DropDownList ID="CmbDeptoNacimiento" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbDepartamentoNac_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                        </asp:DropDownList>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Municipio de Nacimiento*:</asp:Label>
                                        <asp:DropDownList ID="CmbMuncNacimiento" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipioNac_SelectedIndexChanged" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                        </asp:DropDownList>
                                        <br />
                                    </div>






                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Nombres*:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtNombre" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNombre" ErrorMessage="Ingrese su nombre." ForeColor="Red"> </asp:RequiredFieldValidator>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Apellidos*:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtApellido" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtApellido" ErrorMessage="Ingrese su apellido." ForeColor="Red"> </asp:RequiredFieldValidator>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Apellido de casada:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtCasada" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                        <br />
                                    </div>



                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Dirección 1*:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtDireccion" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDireccion" ErrorMessage="Ingrese su dirección." ForeColor="Red"> </asp:RequiredFieldValidator>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Dirección 2:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtDireccion2" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Zona:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtDireccion3" runat="server" TextMode="MultiLine" Rows="2" MaxLength="2" Width="275px" CssClass="form-control" onkeypress="return evitarEnteryNumeros(event)"></asp:TextBox>
                                    </div>



                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">País*:</asp:Label><br />
                                        <asp:DropDownList ID="CmbPais" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbPais_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                        </asp:DropDownList>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                        <asp:DropDownList ID="CmbDepartamento" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbDepartamento_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                        </asp:DropDownList>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                        <asp:DropDownList ID="CmbMunicipio" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipio_SelectedIndexChanged" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();">
                                        </asp:DropDownList>
                                        <br />
                                    </div>



                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Teléfono*:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="txtTelefono" runat="server" MaxLength="24" CssClass="form-control" Width="275px" onblur="validarTelefono(this.value)"></asp:TextBox>
                                        <span id="errorTelefono" style="color: red; font-size: small"></span>
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Correo personal*:</asp:Label>
                                        <br />
                                        <asp:TextBox ID="TxtCorreoPersonal" runat="server" MaxLength="70" CssClass="form-control" Width="275px" onblur="validarCorreo(this.value)"></asp:TextBox>
                                        <span id="errorCorreo" style="color: red; font-size: small"></span>
                                        <br />
                                    </div>

                                    <div class="form-group col-md-4">
                                        <asp:Label runat="server" Font-Bold="true">Estado civil:</asp:Label>
                                        <br />
                                        <asp:DropDownList ID="CmbEstado" runat="server" Width="275px" CssClass="form-control">
                                            <asp:ListItem Selected="False" Value=""></asp:ListItem>
                                            <asp:ListItem>Casado</asp:ListItem>
                                            <asp:ListItem>Soltero</asp:ListItem>
                                            <asp:ListItem>No Consta</asp:ListItem>
                                        </asp:DropDownList>
                                        <br />
                                    </div>
                                </div>
                            </div>

                            <hr />
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-2 mx-auto text-center">
                                    </div>
                                    <div class="col-md-8 mx-auto text-center">
                                        <h5 style="text-align: center;">Información para recibos de pago para futuras transacciones en la universidad</h5>
                                    </div>
                                    <div class="col-md-2 mx-auto text-center">
                                    </div>
                                </div>
                            </div>
                            <div class="container">
                                <div class="row">
                                    <div class="col-md-4 mx-auto text-center">
                                    </div>
                                    <div class="col-md-4 mx-auto text-center">
                                        <asp:Label runat="server">Desea utilizar CF:  </asp:Label>
                                        <br />
                                        <asp:RadioButton ID="RadioButtonNombreSi" runat="server" GroupName="confirmar" Text="SI" />
                                        <asp:RadioButton ID="RadioButtonNombreNo" runat="server" GroupName="confirmar" Text="NO" />
                                    </div>
                                    <div class="col-md-4 mx-auto text-center">
                                    </div>
                                </div>
                            </div>
                            <div id="TableNit">
                                <div class="container">
                                    <div class="row">

                                        <div class="col-md-4 mx-auto text-center">
                                        </div>

                                        <div class="col-md-4 mx-auto text-center">
                                            <asp:Label runat="server" Font-Bold="true">NIT*:</asp:Label>
                                        </div>

                                        <div class="col-md-4 mx-auto text-center">
                                        </div>


                                        <div class="col-md-4 mx-auto text-center">
                                        </div>

                                        <div class="col-md-4 mx-auto text-center d-flex align-items-center justify-content-center">
                                            <asp:TextBox ID="txtNit" runat="server" Width="275px" CssClass="form-control"></asp:TextBox>
                                        </div>

                                        <div class="col-md-4 mx-auto text-center">
                                        </div>


                                        <div class="col-md-4 mx-auto text-center">
                                        </div>

                                        <div class="col-md-4 mx-auto text-center">
                                            <asp:Label runat="server" Font-Size="Small" Text="El NIT no debe de contener guión (-)"></asp:Label>
                                        </div>

                                        <div class="col-md-4 mx-auto text-center">
                                        </div>
                                    </div>
                                </div>

                                <div class="container">
                                    <div class="row">
                                        <div class="col-md-4 mx-auto text-center">
                                        </div>
                                        <div class="col-md-4 mx-auto text-center">
                                            <asp:Button ID="ValidarNIT" runat="server" Text="Validar Nit" CssClass="btn-danger-unis" Enabled="true" OnClick="txtNit_TextChanged" CausesValidation="false" />
                                        </div>
                                        <div class="col-md-4 mx-auto text-center">
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <div class="container">
                                    <div class="row">
                                        <div class="form-group col-md-4">
                                            <asp:Label runat="server" Font-Bold="true">Nombre 1*:</asp:Label>
                                            <br />
                                            <asp:TextBox ID="TxtNombreR" runat="server" Enabled="false" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                            <br />
                                        </div>

                                        <div class="form-group col-md-4">
                                            <asp:Label runat="server" Font-Bold="true">Nombre 2:</asp:Label>
                                            <br />
                                            <asp:TextBox ID="TxtApellidoR" runat="server" Enabled="false" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                            <br />
                                        </div>

                                        <div class="form-group col-md-4">
                                            <asp:Label runat="server" Font-Bold="true">Nombre 3:</asp:Label>
                                            <br />
                                            <asp:TextBox ID="TxtCasadaR" runat="server" Enabled="false" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                            <br />
                                        </div>

                                        <div class="form-group col-md-4">
                                            <asp:Label runat="server" Font-Bold="true">Dirección 1*:</asp:Label>
                                            <asp:TextBox ID="TxtDiRe1" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" Enabled="false" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                            <br />
                                        </div>

                                        <div class="form-group col-md-4">
                                            <asp:Label runat="server" Font-Bold="true">Dirección 2:</asp:Label>
                                            <asp:TextBox ID="TxtDiRe2" runat="server" TextMode="MultiLine" Rows="2" MaxLength="55" Width="275px" CssClass="form-control" Enabled="false" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                            <br />
                                        </div>

                                        <div class="form-group col-md-4">
                                            <asp:Label runat="server" Font-Bold="true">Zona:</asp:Label>
                                            <asp:TextBox ID="TxtDiRe3" runat="server" TextMode="MultiLine" Rows="2" MaxLength="2" Width="275px" CssClass="form-control" Enabled="false" onkeypress="return evitarEnter(event)"></asp:TextBox>
                                            <br />
                                        </div>

                                        <div class="container" id="Combos" runat="server" style="display: none;">
                                            <div class="row">
                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">País*:</asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="CmbPaisNIT" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbPaisNIT_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();"></asp:DropDownList>
                                                    <br />
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="CmbDepartamentoNIT" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CmbDepartamentoNIT_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();"></asp:DropDownList>
                                                    <br />
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                                    <br />
                                                    <asp:DropDownList ID="CmbMunicipioNIT" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipioNIT_SelectedIndexChanged" Width="275px" CssClass="form-control" onchange="mostrarModalEspera();"></asp:DropDownList>
                                                    <br />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="container" id="sustituirCombos" runat="server" style="display: none;">
                                            <div class="row">
                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">País*:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="PaisNit" runat="server" Enabled="false" Width="275px" CssClass="form-control"></asp:TextBox>
                                                    <br />
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="DepartamentoNit" runat="server" Enabled="false" Width="275px" CssClass="form-control"></asp:TextBox>
                                                    <br />
                                                </div>

                                                <div class="form-group col-md-4">
                                                    <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                                    <br />
                                                    <asp:TextBox ID="MunicipioNit" runat="server" Enabled="false" Width="275px" CssClass="form-control"></asp:TextBox>
                                                    <br />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <br />


            <div class="container" id="tbactualizar" runat="server">
                <div class="row">
                    <div class="col-md-4 mx-auto text-center">
                    </div>
                    <div class="col-md-4 mx-auto text-center">
                        <asp:Button ID="BtnActualizar" runat="server" Text="Actualizar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlerta();" OnClick="BtnActualizar_Click" />
                    </div>
                    <div class="col-md-4 mx-auto text-center">
                    </div>
                </div>
            </div>
        </div>

        <div class="modal" id="myModalActualizacion" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="row">
                        <div class="col-md-4 mx-auto text-center">
                        </div>
                        <div class="col-md-4 mx-auto text-center">
                            <div class="modal-spinnerCarne">
                                <div class="spinner"></div>
                            </div>
                        </div>
                        <div class="col-md-4 mx-auto text-center">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 mx-auto text-center">
                            <div class="modal-messageCarne">Por favor, espera mientras la información se está actualizando...</div>
                            <div style="margin-bottom: 20px;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="myModalAlumno" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100vh; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog" role="document" style="display: flex; flex-direction: column; align-items: center;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 style="text-align: center; color: darkred;"><strong>Alerta</strong></h5>
                        <span class="closeAlumno" style="cursor: pointer;">&times;</span>
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

        <div id="myModalNoExisteAlumno" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100vh; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog" role="document" style="display: flex; flex-direction: column; align-items: center;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 style="text-align: center; color: darkred;"><strong>Alerta</strong></h5>
                        <span class="closeNoExisteAlumno" style="cursor: pointer;">&times;</span>
                    </div>
                    <div class="modal-body">
                        <contenttemplate>
                            <div class="container emp-profile">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="profile-head">
                                            <div class="row">
                                                <div class="form-group col-md">
                                                    <asp:Label ID="Label9" runat="server" ForeColor="Black">La persona seleccionada no se encuentra matriculada en un ciclo lectivo vigente</asp:Label>
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

        <div id="myModalPermisos" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100vh; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog" role="document" style="display: flex; flex-direction: column; align-items: center;">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 style="text-align: center; color: darkred;"><strong>Alerta</strong></h5>
                        <span class="closePermisos" style="cursor: pointer;">&times;</span>
                    </div>
                    <div class="modal-body">
                        <contenttemplate>
                            <div class="container emp-profile">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="profile-head">
                                            <div class="row">
                                                <div class="form-group col-md">
                                                    <asp:Label ID="Label10" runat="server" ForeColor="Black">No cuenta con los permisos correspondientes para visualizar la información de dicho estudiante</asp:Label>
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

        <div class="modal" id="myModalCorrecto" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="row">
                        <div class="col-md-12 mx-auto text-center">
                            <div style="margin-bottom: 20px;"></div>
                            <div class="modal-messageCarne">Su información fue actualizada correctamente.</div>
                            <div style="margin-bottom: 20px;"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal" id="myModalEspera" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="row">
                        <div class="col-md-4 mx-auto text-center">
                        </div>
                        <div class="col-md-4 mx-auto text-center">
                            <div class="modal-spinnerCarne">
                                <div class="spinner"></div>
                            </div>
                        </div>
                        <div class="col-md-4 mx-auto text-center">
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 mx-auto text-center">
                            <div class="modal-messageCarne">Por favor, espera un momento</div>
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

        <div id="myModalBusquedaMultiple" class="modal" style="background: rgba(0, 0, 0, 0.5); display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; justify-content: center; align-items: center; z-index: 9999;">
            <div class="modal-dialog modal-xl" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 style="text-align: center; color: darkred; text-align: center"><strong>Información de Busqueda</strong></h4>
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
                                                    <asp:Button ID="BtnAceptarBusqueda" runat="server" Text="Aceptar" CssClass="btn-danger-unis" Enabled="true" CausesValidation="false" OnClick="BtnAceptarBusqueda_Click" />
                                                    <br />
                                                </div>
                                            </div>
                                            <div class="row">
                                                <div class="form-group col-md" style="max-height: calc(100vh - 150px); overflow-y: auto;">
                                                    <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
                                                    <div class="containerGV" id="GVContainer">
                                                        <asp:GridView ID="GridViewBusqueda" runat="server" AutoGenerateColumns="false"
                                                            CssClass="table table-condensed table-bordered ">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Seleccionar" ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:RadioButton ID="RBBusqueda" runat="server" GroupName="BusquedaGroup" OnClick="selectOnlyThis(this)" />
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


        <div class="container" runat="server">
            <div class="row">
                <div class="col-md-2">
                </div>
                <div class="col-md-8 d-flex justify-content-center align-items-center">
                    <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"></asp:Label>
                </div>
                <div class="col-md-2">
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-4 mx-auto text-center">
                </div>
                <div class="col-md-4 mx-auto text-center d-flex justify-content-center align-items-center">
                    <asp:Button ID="BtnDownload" runat="server" Text="Descargar Manual" CssClass="btn-danger-unis" OnClick="BtnDownload_Click" Style="display: none" />
                </div>
                <div class="col-md-4 mx-auto text-center">
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-md-4 mx-auto text-center">
                </div>
                <div class="col-md-4 mx-auto text-center d-flex justify-content-center align-items-center">
                    <asp:Button ID="BtnReload" runat="server" Text="Recargar Página" CssClass="btn-danger-unis" OnClick="BtnReload_Click" Style="display: none" />
                </div>
                <div class="col-md-4 mx-auto text-center">
                </div>
            </div>
        </div>

        


        <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">

        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
        <script src="https://cdn.datatables.net/v/dt/dt-1.13.6/datatables.min.js"></script>
    </div>
    <script>
        var userAgent = navigator.userAgent;

        function validarCorreo(correo) {
            var errorCorreoElement = document.getElementById("errorCorreo");
            var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

            if (correo.trim() === "") {
                errorCorreoElement.textContent = "Ingrese su correo personal.";
            } else if (!regex.test(correo)) {
                errorCorreoElement.textContent = "El formato del correo electrónico no es válido.";
            } else {
                errorCorreoElement.textContent = " ";
            }
        }

        function validarTelefono(Telefono) {
            var errorTelefonoElement = document.getElementById("errorTelefono");

            if (Telefono.trim() === "") {
                errorTelefonoElement.textContent = "Ingrese su teléfono.";
            } else if (Telefono.length > 0 && Telefono.length <= 7) {
                errorTelefonoElement.textContent = "El número de télefono debe de tener 8 caracteres";
            } else {
                errorTelefonoElement.textContent = "";
            }
        }

        function mostrarAlerta() {
            var mensaje = "";
            var apellido = document.getElementById('<%= txtApellido.ClientID %>').value;
            var nombre = document.getElementById('<%= txtNombre.ClientID %>').value;
            var nombreR = document.getElementById('<%= TxtNombreR.ClientID %>').value;
            var nit = document.getElementById('<%= txtNit.ClientID %>').value;
            var direccion1 = document.getElementById('<%= txtDireccion.ClientID %>').value;
            var direccionR1 = document.getElementById('<%= TxtDiRe1.ClientID %>').value;
            var telefono = document.getElementById('<%= txtTelefono.ClientID %>').value;
            var pais = document.getElementById('<%= CmbPais.ClientID %>').value;
            var depto = document.getElementById('<%= CmbDepartamento.ClientID %>').value;
            var muni = document.getElementById('<%= CmbMunicipio.ClientID %>').value;
            var paisN = document.getElementById('<%= CmbPaisNIT.ClientID %>').value;
            var deptoN = document.getElementById('<%= CmbDepartamentoNIT.ClientID %>').value;
            var muniN = document.getElementById('<%= CmbMunicipioNIT.ClientID %>').value;
            var Correo = document.getElementById('<%= TxtCorreoPersonal.ClientID %>').value;

            // Obtener el valor del <textarea>
            var ValidacionNit = $('#<%= ValidacionNit.ClientID %>').val().trim();
            var TrueNit = $('#<%= TrueNit.ClientID %>').val().trim();
            var txtNombre = $('#<%= txtNombre.ClientID %>').val().trim();
            var txtNInicial = $('#<%= txtNInicial.ClientID %>').val().trim();
            var ControlCF2 = $('#<%= ControlCF2.ClientID %>').val().trim();
            var txtApellido = $('#<%= txtApellido.ClientID %>').val().trim();
            var txtAInicial = $('#<%= txtAInicial.ClientID %>').val().trim();
            var txtCasada = $('#<%= txtCasada.ClientID %>').val().trim();
            var txtCInicial = $('#<%= txtCInicial.ClientID %>').val().trim();
            var modal = document.getElementById("myModalActualizacion");
            var divCombos = $('#<%= Combos.ClientID %>');
            var files = fileUpload.files;


            if (TrueNit !== nit && nit !== "CF") {
                // Realiza las acciones necesarias si el valor es diferente de cero
                alert("El NIT ha cambiado, es necesario validar.");
                return false;
            } else {
                if (apellido.trim() === "") {
                    mensaje = "-Los Apellidos son requerido.";
                }

                if (nombre.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-Los Nombres son requeridos.";
                    } else {
                        mensaje = mensaje + "\n-Los Nombres son requeridos.";
                    }
                }

                if (direccion1.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-La Dirección 1 es requerida.";
                    } else {
                        mensaje = mensaje + "\n-La Dirección 1 es requerida.";
                    }
                }

                if (pais.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El país es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El país es requerido.";
                    }
                }

                if (depto.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El departamento es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El departamento es requerido.";
                    }
                }

                if (muni.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El municipio es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El municipio es requerido.";
                    }
                }

                if (Correo.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El Correo Personal es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El Correo Personal es requerido.";
                    }
                }

                if (telefono.length > 0 && telefono.length <= 7) {
                    if (mensaje.trim() == "") {
                        mensaje = "-El Teléfono debe de tener 8 carácteres.";
                    } else {
                        mensaje = mensaje + "\n-El Teléfono debe de tener 8 carácteres.";
                    }
                }

                if (telefono.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El Teléfono es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El Teléfono es requerido.";
                    }

                }


                if (nit.trim() === "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El NIT para el recibo es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El NIT para el recibo es requerido.";
                    }
                }

                if (direccionR1.trim() === "" && nombreR.trim() !== "") {
                    if (mensaje.trim() == "") {
                        mensaje = "-La Dirección 1 para el recibo es requerida.";
                    } else {
                        mensaje = mensaje + "\n-La Dirección 1 para el recibo es requerida.";
                    }
                }



                if (paisN.trim() === "" && ControlCF2.trim() === "2") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El país para el recibo es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El país para el recibo es requerido.";
                    }
                }

                if (deptoN.trim() === "" && ControlCF2.trim() === "2") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El departamento para el recibo es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El departamento para el recibo es requerido.";
                    }
                }

                if (muniN.trim() === "" && ControlCF2.trim() === "2") {
                    if (mensaje.trim() == "") {
                        mensaje = "-El municipio para el recibo es requerido.";
                    } else {
                        mensaje = mensaje + "\n-El municipio para el recibo es requerido.";
                    }
                }

                if (mensaje.trim() !== "") {
                    mensaje = mensaje.replace("/\n/g", "<br>");
                    alert(mensaje);
                    return false;
                } else if (confirm("¿Está seguro de que su información es correcta?")) {                   
                    $('#myModalActualizacion').css('display', 'block');
                    __doPostBack('<%= BtnActualizar.ClientID %>', '');
                    return true; // Permite continuar con la acción del botón
                } else {
                    return false; // Cancela la acción del botón
                }
            }
        }

        
        function NoExiste() {
            $('#myModalAlumno').css('display', 'block');
            $('#myModalBusquedaMultiple').css('display', 'none');
            return false;
        }

        function NoExisteAlumno() {
            $('#myModalNoExisteAlumno').css('display', 'block');
            return false;
        }

        function NoTienePermisos() {
            $('#myModalPermisos').css('display', 'block');
            return false;
        }

        function NoExisteNit() {
            alert("El NIT no existe. Intente de nuevo");
        }

        function ConfirmacionActualizacionSensible() {
            mensaje = "Su información fue almacenada correctamente. \nLa información ingresada debe ser aprobada antes de ser confirmada.\nActualmente, solo se muestran los datos que han sido previamente confirmados.";
            mensaje = mensaje.replace("/\n/g", "<br>");
            alert(mensaje);
            window.location.href = "ActualizacionGeneralEstudiantes.aspx";
        }

        function ConfirmacionActualizacion() {
            mensaje = "Su información fue actualizada correctamente.";
            mensaje = mensaje.replace("/\n/g", "<br>");
            alert(mensaje);
            window.location.href = "ActualizacionGeneralEstudiantes.aspx";
        }

        $(document).ready(function () {
            // Function to add the code
            function RBSi() {
                $('#<%= RadioButtonNombreSi.ClientID %>').on('change', function () {
                    if ($(this).is(':checked')) {
                        $('#<%= ControlCF.ClientID %>').val(" ");
                        $('#<%= ControlCF2.ClientID %>').val("1");
                        $('#<%= TxtNombreR.ClientID %>').val($('#<%= txtNombre.ClientID %>').val());
                        $('#<%= TxtApellidoR.ClientID %>').val($('#<%= txtApellido.ClientID %>').val());
                        $('#<%= TxtCasadaR.ClientID %>').val($('#<%= txtCasada.ClientID %>').val());
                        $('#<%= TxtDiRe1.ClientID %>').val($('#<%= txtDireccion.ClientID %>').val());
                        $('#<%= TxtDiRe2.ClientID %>').val($('#<%= txtDireccion2.ClientID %>').val());
                        $('#<%= TxtDiRe3.ClientID %>').val($('#<%= txtDireccion3.ClientID %>').val());
                        $('#<%= PaisNit.ClientID %>').val($('#<%= CmbPais.ClientID %>').val());
                        $('#<%= MunicipioNit.ClientID %>').val($('#<%= CmbMunicipio.ClientID %>').val());
                        $('#<%= DepartamentoNit.ClientID %>').val($('#<%= CmbDepartamento.ClientID %>').val());
                        $('#<%= StateNIT.ClientID %>').val($('#<%= State.ClientID %>').val());
                        $('#<%= txtNit.ClientID %>').val('CF');
                        $('#<%= txtNit.ClientID %>').prop('disabled', true);
                        $('#<%= ValidarNIT.ClientID %>').prop('disabled', true);
                        $('#<%= TxtDiRe1.ClientID %>').prop('disabled', true);
                        $('#<%= TxtDiRe2.ClientID %>').prop('disabled', true);
                        $('#<%= TxtDiRe3.ClientID %>').prop('disabled', true);
                        $('#<%= PaisNit.ClientID %>').prop('disabled', true);
                        $('#<%= MunicipioNit.ClientID %>').prop('disabled', true);
                        $('#<%= DepartamentoNit.ClientID %>').prop('disabled', true);
                        $('#<%= lblActualizacion.ClientID %>').text('');

                        // Hacer visible la fila Combos
                        $('#<%= Combos.ClientID %>').hide();
                        // Hacer visible la fila sustitucion de Combos
                        $('#<%= sustituirCombos.ClientID %>').hide();
                    }
                });
            }

            // Call the function
            RBSi();
        });

        $(document).ready(function () {
            // Capturar el cambio de estado del RadioButton
            $('#<%= RadioButtonNombreSi.ClientID %>').change(function () {
                if ($(this).is(':checked')) {
                    // El RadioButton ha sido marcado, ocultar la fila
                    $('#<%= Combos.ClientID %>').hide();
                    $('#<%= sustituirCombos.ClientID %>').show();
                } else {
                    // El RadioButton ha sido desmarcado, mostrar la fila
                    $('#<%= Combos.ClientID %>').show();
                    $('#<%= sustituirCombos.ClientID %>').hide();
                }
            });

            // Verificar el estado inicial del RadioButton al cargar la página
            if ($('#<%= RadioButtonNombreSi.ClientID %>').is(':checked')) {
                // El RadioButton está marcado, ocultar la fila
                $('#<%= Combos.ClientID %>').hide();
                $('#<%= sustituirCombos.ClientID %>').show();
            } else {
                // El RadioButton no está marcado, mostrar la fila
                $('#<%= Combos.ClientID %>').show();
                $('#<%= sustituirCombos.ClientID %>').hide();
            }
        });

        $(document).ready(function () {
            // Function to add the code
            function RBNo() {
                $('#<%= RadioButtonNombreNo.ClientID %>').on('change', function () {
                    if ($(this).is(':checked')) {
                        $('#<%= ControlCF.ClientID %>').val(" ");
                        $('#<%= ControlCF2.ClientID %>').val("2");
                        $('#<%= TxtNombreR.ClientID %>').val("");
                        $('#<%= TxtApellidoR.ClientID %>').val("");
                        $('#<%= TxtCasadaR.ClientID %>').val("");
                        $('#<%= txtNit.ClientID %>').val("");
                        $('#<%= TxtDiRe1.ClientID %>').val("");
                        $('#<%= TxtDiRe2.ClientID %>').val("");
                        $('#<%= TxtDiRe3.ClientID %>').val("");
                        $('#<%= CmbPaisNIT.ClientID %>').val("");
                        $('#<%= CmbDepartamentoNIT.ClientID %>').val("");
                        $('#<%= CmbMunicipioNIT.ClientID %>').val("");
                        $('#<%= StateNIT.ClientID %>').val("");
                        $('#<%= txtNit.ClientID %>').prop('disabled', false);
                        $('#<%= ValidarNIT.ClientID %>').prop('disabled', false);
                        $('#<%= TxtDiRe1.ClientID %>').prop('disabled', false);
                        $('#<%= TxtDiRe2.ClientID %>').prop('disabled', false);
                        $('#<%= TxtDiRe3.ClientID %>').prop('disabled', false);
                        $('#<%= CmbPaisNIT.ClientID %>').prop('disabled', false);
                        $('#<%= CmbDepartamentoNIT.ClientID %>').prop('disabled', false);
                        $('#<%= CmbMunicipioNIT.ClientID %>').prop('disabled', false);
                        $('#<%= PaisNit.ClientID %>').val($('#<%= CmbPaisNIT.ClientID %>').val());
                        // Hacer visible la fila Combos
                        $('#<%= Combos.ClientID %>').show();
                        $('#<%= sustituirCombos.ClientID %>').hide();

                        var deptos = document.getElementById('<%= CmbDepartamentoNIT.ClientID %>');
                        var muni = document.getElementById('<%= CmbMunicipioNIT.ClientID %>');
                        while (deptos.options.length > 0) {
                            deptos.remove(0);
                        }
                        while (muni.options.length > 0) {
                            muni.remove(0);
                        }
                    }
                });
            }

            // Call the function
            RBNo();

        });



        $('.closeAlumno').click(function () {
            $('#myModalAlumno').css('display', 'none');
        });

        $('.closeNoExisteAlumno').click(function () {
            $('#myModalNoExisteAlumno').css('display', 'none');
        });

        $('.closePermisos').click(function () {
            $('#myModalPermisos').css('display', 'none');
            $('#myModalBusquedaMultiple').css('display', 'block');
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO NOMBRE EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtNombre.ClientID %>').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= TxtNombreR.ClientID %>').val($('#<%= txtNombre.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO APELLIDO EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtApellido.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= TxtApellidoR.ClientID %>').val($('#<%= txtApellido.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO APELLIDO DE CASADA EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtCasada.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= TxtCasadaR.ClientID %>').val($('#<%= txtCasada.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE LA MISMA DIRECCION 1 EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtDireccion.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= TxtDiRe1.ClientID %>').val($('#<%= txtDireccion.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE LA MISMA DIRECCION 2 EN EL RECIBO 2.0
        $(document).ready(function () {
            $('#<%= txtDireccion2.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= TxtDiRe2.ClientID %>').val($('#<%= txtDireccion2.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE LA MISMA DIRECCION 3 EN EL RECIBO 
        $(document).ready(function () {
            $('#<%= txtDireccion3.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= TxtDiRe3.ClientID %>').val($('#<%= txtDireccion3.ClientID %>').val());
                }
            });
        });

        //FUNCION QUE PERMITE QUE SE INGRESE EL MISMO PAIS EN EL RECIBO
        $(document).ready(function () {
            $('#<%= CmbPaisNIT.ClientID %> ').on('input', function () {
                if (!$('#<%= RadioButtonNombreNo.ClientID %>').is(':checked') && $('#<%= ControlCF.ClientID %>').val().trim() != 'CF') {
                    $('#<%= PaisNit.ClientID %>').val($('#<%= CmbPaisNIT.ClientID %>').val());
                }
            });
        });

        function VerificarCantidadTelefono(sender, args) {
            args.IsValid = (args.Value.length >= 7);
        }


        //FUNCION PARA EVITAR QUE SEA INGRESADO EL -
        $(document).ready(function () {
            $('#<%= txtNit.ClientID %>').on('keypress', function (event) {
                var keyCode = event.which || event.keyCode;
                var character = String.fromCharCode(keyCode);
                if (character === '-') {
                    event.preventDefault();
                    alert('No se permite el ingreso del guión (-)');
                } else if (character === 'C' || character === 'c') {
                    event.preventDefault();
                    alert('Para ingresar CF, seleccione la casilla NO, que se encuentra arriba del campo del ingreso del NIT');
                }
            });
        });

        //Valdar Numeros de telefono
        $(document).ready(function () {
            $('#<%= txtTelefono.ClientID %>').on('input', function () {
                var inputValue = $(this).val();
                var numericValue = inputValue.replace(/[^0-9]/g, '');
                $(this).val(numericValue);
            });

            $('#<%= txtTelefono.ClientID %>').on('keypress', function (event) {
                var keyCode = event.which || event.keyCode;
                var character = String.fromCharCode(keyCode);

                if (!/^[0-9]+$/.test(character)) {
                    event.preventDefault();
                    alert('Solo se permiten ingresar números.');
                }
            });
        });

        //Detectar cambio de nit
        $(document).ready(function () {
            $('#<%= txtNit.ClientID %>').on('input', function () {
                var txtNit = $('#<%= txtNit.ClientID %>').val().trim();
                var TrueNit = $('#<%= TrueNit.ClientID %>').val().trim();
                var labelValidacion = $('#<%= ValidacionNit.ClientID %>').val().trim();


                if (txtNit !== TrueNit || txtNit !== 'CF') {
                    $('#<%= ValidacionNit.ClientID %>').val("1");
                } else {
                    $('#<%= ValidacionNit.ClientID %>').val("0");
                    TrueNit.text(txtNit);
                }
            });
        });



        function mostrarModalEspera() {
            var modal = document.getElementById("myModalEspera");
            modal.style.display = "block";
        }

        function ocultarModalEspera() {
            var modal = document.getElementById("myModalEspera");
            modal.style.display = "none";
        }

        function ocultarModalActualizacion() {
            var modal = document.getElementById("myModalActualizacion");
            modal.style.display = "none";
        }

        function mostrarModalCorrecto() {
            var modal = document.getElementById("myModalCorrecto");
            modal.style.display = "block";
            var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
            lblActualizacion.html("");

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
                window.location.href = "ActualizacionGeneralEstudiantes.aspx";
            }, 4000); // 4000 milisegundos =  segundos
        }

        function mostrarModalError() {
            var modal = document.getElementById("myModalError");
            modal.style.display = "block";
            var lblActualizacion = $("#<%= lblActualizacion.ClientID %>");
            lblActualizacion.html("");

            setTimeout(function () {
                modal.style.display = "none"; // Oculta el modal después de 10 segundos
                window.location.href = "ActualizacionGeneralEstudiantes.aspx";
            }, 4000); // 4000 milisegundos =  segundos
        }

        //evitar enter y letras, permite ingresar solo numeros
        function evitarEnteryNumeros(e) {
            var charCode = (e.which) ? e.which : event.keyCode;

            // Verifica si la tecla presionada es un número y no es Enter
            if ((charCode >= 48 && charCode <= 57) && charCode !== 13) {
                return true;
            }

            return false;
        }


        function evitarEnter(e) {
            var charCode = (e.which) ? e.which : event.keyCode;

            // Verifica si la tecla presionada es un número y no es Enter
            if (charCode !== 13) {
                return true;
            }

            return false;
        }


        document.addEventListener("DOMContentLoaded", function () {
            // Get the modal
            var modal = document.getElementById("myModalAlumno");

            // Get the button that opens the modal
            var btn = document.getElementById("openModalBtn"); // Assuming you have a button with this ID

            // Get the <span> element that closes the modal
            var span = document.getElementsByClassName("closeAlumno")[0];

            // When the user clicks on the button, open the modal
            btn.onclick = function () {
                modal.style.display = "flex";
            }

            // When the user clicks on <span> (x), close the modal
            span.onclick = function () {
                modal.style.display = "none";
            }

            // When the user clicks anywhere outside of the modal, close it
            window.onclick = function (event) {
                if (event.target == modal) {
                    modal.style.display = "none";
                }
            }
        });

        
        function Busqueda() {
            $('#myModalBusquedaMultiple').css('display', 'block');
            return false;
        }

        $('.closeBusqueda').click(function () {
            $('#myModalBusquedaMultiple').css('display', 'none');
        });

        function selectOnlyThis(radioButton) {
            var allRadios = document.querySelectorAll('[id*="RBBusqueda"]');
            allRadios.forEach(function (radio) {
                radio.checked = false;
            });
            radioButton.checked = true;
        }

        function selectOnlyThisDoc(radioButton) {
            var allRadios = document.querySelectorAll('[id*="RBDocPrincipal"]');
            allRadios.forEach(function (radio) {
                radio.checked = false;
            });
            radioButton.checked = true;
        }

        $(document).ready(function () {
            $('#GridViewDocumentos tr').each(function () {
                var tipoDocumento = $(this).find('td:eq(2)').text();
                if (tipoDocumento == "DPI" || tipoDocumento == "Pasaporte") {
                    $(this).css('background-color', '#CCCCCC');
                }
            });
        });
    </script>


    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>


