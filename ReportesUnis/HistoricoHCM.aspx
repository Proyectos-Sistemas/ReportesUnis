<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricoHCM.aspx.cs" Inherits="ReportesUnis.HistoricoHCM"  %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <br />
                    <h2 style="text-align:center;">
                   Históricos HCM</h2>
        <hr />
        <br />
        <div class="container">
            <div class="row">

                 <div class="form-group col-md-4">
                    <asp:Label Text="Número de persona" for="PersonNumber" runat="server" />
                    <asp:TextBox ID="PersonNumber" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Nombres" for="Nombres" runat="server" />
                    <asp:TextBox ID="Nombres" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Apellidos" for="Apellidos" runat="server" />
                    <asp:TextBox ID="Apellidos" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Número de identificador nacional" for="NationalIdentifier" runat="server" />
                    <asp:TextBox ID="NationalIdentifier" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Nombre de departamento" for="Departamento" runat="server" />
                    <asp:DropDownList ID="Departamento" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">Seleccione...</asp:ListItem>
                    </asp:DropDownList>
                    
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Fecha de nacimiento" for="FechaNacimiento" runat="server" />
                    <asp:TextBox ID="FechaNacimiento" CssClass="form-control" runat="server" TextMode="Date" autocomplete="off"></asp:TextBox>
                </div>

            </div>
            <div class="row">
                <div class="col-md-4 align-self-center">
                    <asp:Button ID="ButtonAceptar" runat="server" Text="Buscar" type="submit" CssClass="btn btn-danger-unis btn-buscar-historico"  OnClick="ButtonAceptar_Click"/>
                </div>
            </div>
        </div>
        <hr />
        <br />
        <div class="container-fluid">
            <div class="row">
                    <div class="col-md-12" >
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered"  OnRowCommand="GridView1_RowCommand"  DataKeyNames="idRegistro" ShowHeaderWhenEmpty="true" >
                            <Columns>
                                <asp:ButtonField CommandName="cmdDetalle" ControlStyle-CssClass="btn btn-danger-unis btn-detalleUNIS" ButtonType="Button"  Text="Detalle" HeaderText="Detalle asignación" />

                                <asp:TemplateField HeaderText="Número de asignación">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("assignmentnumber")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Número de persona">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("personnumber")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Primer apellido">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("lastname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Segundo apellido">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("previouslastname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Apellido de casada">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("nameinformation1")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Primer nombre">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("firstname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Segundo nombre">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("middlenames")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Nombre de departamento">
                                    <ItemTemplate>
                                        <div style="width: 350px;">
                                            <%# Eval("departmentname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Fecha de inicio efectiva">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("effectivestartdate","{0:yyyy-MM-dd}")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Fecha de finalización efectiva">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("effectiveenddate","{0:yyyy-MM-dd}")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Fecha de nacimiento">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("dateofbirth","{0:yyyy-MM-dd}")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

  
                                <asp:TemplateField HeaderText="Estado civil">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("maritalstatus")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Sexo">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("sex")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Etnia">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("ethnicity")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Religión">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("religion")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo de sangre">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("bloodtype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Municipio de nacimiento">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("townofbirth")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="País de nacimiento">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("countryofbirth")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Departamento de nacimiento">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("regionofbirth")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de legislación">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("legislationcode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Nivel de educación">
                                    <ItemTemplate>
                                        <div style="width: 350px;">
                                            <%# Eval("highesteducationlevel")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Dirección línea 1">
                                    <ItemTemplate>
                                        <div style="width: 350px; ">
                                            <%# Eval("addressline1")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Zona">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("addladdressattribute3")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Código postal">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("postalcode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Pueblo o ciudad">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("townorcity")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="País">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("country")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo de dirección">
                                    <ItemTemplate>
                                        <div style="width: 200px;">
                                            <%# Eval("addresstype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Dirección de correo electrónico">
                                    <ItemTemplate>
                                        <div style="width: 225px;">
                                            <%# Eval("emailaddress")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Tipo de correo electrónico">
                                    <ItemTemplate>
                                        <div style="width: 250px;">
                                            <%# Eval("emailtype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Número teléfonico">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("phonenumber")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo de télefono">
                                    <ItemTemplate>
                                        <div style="width: 200px;">
                                            <%# Eval("phonetype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo de trabajador">
                                    <ItemTemplate>
                                        <div style="width: 200px;">
                                            <%# Eval("workertype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de acción">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("actioncode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Categoría de asignación">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("assignmentcategory")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
								
                                <asp:TemplateField HeaderText="Categoría de trabajador">
                                    <ItemTemplate>
                                        <div style="width: 250px;">
                                            <%# Eval("workercategory")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de salario por hora">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("hourlysalariedcode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Nombre de salario por hora">
                                    <ItemTemplate>
                                        <div style="width: 200px;">
                                            <%# Eval("hourlysalariedname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de grado">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("gradecode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Nombre de grado">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("gradename")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de posición">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("positioncode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Nombre de posición">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("positionname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de trabajo">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("jobcode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Nombre de trabajo">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("jobname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
								
                                <asp:TemplateField HeaderText="Código de localización">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("locationcode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Frecuencia">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("frequency")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Horas normales">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("normalhours")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tiempo parcial completo">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("fullparttime")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código de tipo de persona">
                                    <ItemTemplate>
                                        <div style="width: 200px;">
                                            <%# Eval("persontypecode")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Nombre de banco">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("bankname")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Número de cuenta">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("accountnumber")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo de cuenta">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("accounttype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                                
                        </asp:GridView>


                    </div>

                    <!-- Modal -->
                    <div class="modal fade" id="currentdetail" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                      <div class="modal-dialog modal-xl" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLabel">Detalle</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                              <span aria-hidden="true">&times;</span>
                            </button>
                          </div>
                          <div class="modal-body">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>

                                <div class="container emp-profile">
                                    <form method="post">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="profile-head">
                                                            <h5>
                                                              <b>Nombre:</b> <span id="NombreEmpleado"></span>
                                                            </h5>
                                                            <h6>
                                                               <b>Dirección:</b> <span id="DireccionEmpleado"></span>
                                                            </h6>
                                                             <h6>
                                                               <b>Número de asignación:</b> <span id="CodigoAsignacion"></span>
                                                            </h6>
                                                            <br />
                                                            <ul class="nav nav-tabs" id="myTab" role="tablist">
                                                                <li class="nav-item">
                                                                    <a class="nav-link active" id="RelacionLaboral-tab" data-toggle="tab" href="#RelacionLaboral" role="tab" aria-controls="RelacionLaboral" aria-selected="true">Relación laboral</a>
                                                                </li>
                                                                <li class="nav-item">
                                                                    <a class="nav-link" id="InfoPersonal-tab" data-toggle="tab" href="#InfoPersonal" role="tab" aria-controls="InfoPersonal" aria-selected="false">Información personal</a>
                                                                </li>
                                                                <li class="nav-item">
                                                                    <a class="nav-link" id="MetodosComunicacion-tab" data-toggle="tab" href="#MetodosComunicacion" role="tab" aria-controls="MetodosComunicacion" aria-selected="false">Métodos de comunicación</a>
                                                                </li>
                                                                <li class="nav-item">
                                                                    <a class="nav-link" id="Identificadores-tab" data-toggle="tab" href="#Identificadores" role="tab" aria-controls="Identificadores" aria-selected="false">Identificadores</a>
                                                                </li>
                                                                <li class="nav-item">
                                                                    <a class="nav-link" id="Salarios-tab" data-toggle="tab" href="#Salarios" role="tab" aria-controls="Salarios" aria-selected="false">Salarios</a>
                                                                </li>
                                                                <li class="nav-item">
                                                                    <a class="nav-link" id="DatosBancarios-tab" data-toggle="tab" href="#DatosBancarios" role="tab" aria-controls="DatosBancarios" aria-selected="false">Datos bancarios</a>
                                                                </li>
                                                            </ul>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="tab-content profile-tab" id="myTabContent">
                                                    <div class="tab-pane fade  show active" id="RelacionLaboral" role="tabpanel" aria-labelledby="RelacionLaboral-tab">
                                                        <br />
                                                        <h4>Relación laboral</h4>
                                                        <a><b>Tipo de trabajador: </b><span id="sTipoDeTrabajador"></span></a>
                                                        <br />
                                                        <a><b>Puesto de trabajador: </b><span id="sPuestoDeTrabajador"></span></a>
                                                        <br />
                                                        <a><b>Grado de trabajador: </b><span id="sGradoDeTrabajador"></span></a>
                                                        <br />
                                                        <a><b>Departamento de trabajo: </b><span id="sDepartamentoDeTrabajador"></span></a>
                                                        <br />
                                                        <br />
                                                        <h4>Detalle de contrato</h4>
                                                        <a><b>Tipo: </b> <span id="sTipoDeContrato"></span></a>
                                                        <br />
                                                        <a><b>Fecha de cese: </b><span id="sFCeseDeContrato"></span></a>
                                                        <br />
                                                        <a><b>Fecha de inicio de vigencia: </b><span id="sFInicioDeContrato"></a>
                                                        <br />
                                                        <a><b>Fecha fin de vigencia: </b><span id="sFFinDeContrato"></a>

                                                    </div>
                                                    <div class="tab-pane fade" id="InfoPersonal" role="tabpanel" aria-labelledby="InfoPersonal-tab">
                                                        <br />
                                                        <h4>Información biográfica</h4>
                                                        <a><b>Fecha de nacimiento: </b><span id="sFechaNacimiento"></span></a>
                                                        <br />
                                                        <a><b>País de nacimiento: </b><span id="sPaisNacimiento"></span></a>
                                                        <br />
                                                        <a><b>Región de nacimiento: </b><span id="sRegionNacimiento"></span></a>
                                                        <br />
                                                        <a><b>Municipio de nacimiento: </b><span id="sMuniciioNacimiento"></span></a>
                                                        <br />
                                                        <a><b>Grupo sanguíneo: </b><span id="sGrupoSanguineo"></span></a>
                                                        <br /><br />
                                                        <h4>Información legisltiva</h4>
                                                        <a><b>Sexo: </b> <span id="sSexo"></span></a>
                                                        <br />
                                                        <a><b>Estado civil: </b><span id="sEstadoCivil"></span></a>
                                                        <br />
                                                        <a><b>Nivel de educación: </b><span id="sNivelEducacion"></a>
                                                        <br />
                                                        <a><b>Origen étnico: </b><span id="sOrigenEtnico"></a>
                                                        <br />
                                                        <a><b>Religión: </b><span id="sReligion"></a>
                                                    </div>
                                                    <div class="tab-pane fade" id="MetodosComunicacion" role="tabpanel" aria-labelledby="MetodosComunicacion-tab">
                                                         <br />
                                                        <div class="row">
                                                            <div class="col-md-6">
                                                                <h4>Teléfonos</h4>
                                                                <table class="table" id="tblNumeroTelefono">
                                                                      <thead>
                                                                        <tr>
                                                                          <th scope="col">#</th>
                                                                          <th scope="col">Tipo de teléfono</th>
                                                                          <th scope="col">Teléfono</th>
                                                                        </tr>
                                                                      </thead>
                                                                      <tbody>

                                                                      </tbody>
                                                                    </table>
                                                            </div>
                                                            <div class="col-md-6">
                                                                <h4>Correos electrónicos</h4>
                                                                <table class="table" id="tblCorreoElectronico">
                                                                  <thead>
                                                                    <tr>
                                                                      <th scope="col">#</th>
                                                                      <th scope="col">Tipo de correo electrónico</th>
                                                                      <th scope="col">Correo electrónico</th>
                                                                    </tr>
                                                                  </thead>
                                                                  <tbody>

                                                                  </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="tab-pane fade" id="Identificadores" role="tabpanel" aria-labelledby="Identificadores-tab">
                                                        <br />
                                                        <div class="row">
                                                            <div class="col-md-6">
                                                                <h4>Identificadores nacionales</h4>
                                                                <table class="table" id="tblIdenNacional">
                                                                    <thead>
                                                                    <tr>
                                                                        <th scope="col">#</th>
                                                                        <th scope="col">Tipo de identificador nacional</th>
                                                                        <th scope="col">Identificador nacional</th>
                                                                    </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                            <div class="col-md-6">
                                                                <h4>Identificadores externos</h4>
                                                                <table class="table" id="tblIdenExterno">
                                                                  <thead>
                                                                    <tr>
                                                                      <th scope="col">#</th>
                                                                      <th scope="col">Tipo de identificador externo</th>
                                                                      <th scope="col">Identificador externo</th>
                                                                    </tr>
                                                                  </thead>
                                                                  <tbody>

                                                                  </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="tab-pane fade" id="Salarios" role="tabpanel" aria-labelledby="Salarios-tab">
                                                        <br />
                                                        <table class="table" id="tblSalarios">
                                                          <thead>
                                                            <tr>
                                                              <th scope="col">#</th>
                                                              <th scope="col">Fecha inicio</th>
                                                              <th scope="col">Fecha fin</th>
                                                              <th scope="col">Salario</th>
                                                            </tr>
                                                          </thead>
                                                          <tbody>

                                                          </tbody>
                                                        </table>
                                                    </div>
                                                    <div class="tab-pane fade" id="DatosBancarios" role="tabpanel" aria-labelledby="DatosBancarios-tab">
                                                        <br />
                                                        <a><b>Nombre del banco: </b><span id="sNombreBanco"></span></a>
                                                        <br />
                                                        <a><b>Número de cuenta: </b><span id="sNumeroCuenta"></span></a>
                                                        <br />
                                                        <a><b>Tipo de centa: </b><span id="sTipoCuenta"></span></a>
                                                        <br />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </form>           
                                </div>

  

                                    </span></span></span></span></span>

  

                               </ContentTemplate>
                               <Triggers>
                                   <asp:AsyncPostBackTrigger ControlID="GridView1"  EventName="RowCommand" />  
                               </Triggers>
                               </asp:UpdatePanel> 
                          </div>
                          <div class="modal-footer">
                            <button type="button" class="btn btn-danger-unis" data-dismiss="modal">Cerrar</button>
                          </div>
                        </div>
                      </div>
                    </div>
            
            </div>
        </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>    
</asp:Content>