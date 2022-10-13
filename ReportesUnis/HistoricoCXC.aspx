<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricoCXC.aspx.cs" Inherits="ReportesUnis.HistoricoCXC"  %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <br />
                    <h2 style="text-align:center;">
                   Históricos Cuentas por cobrar</h2>
        <hr />
        <br />
        <div class="container">
            <div class="row">

                 <div class="form-group col-md-4">
                    <asp:Label Text="Unidad de Negocio" for="UnidadDeNegocio" runat="server" />
                    <asp:TextBox ID="UnidadDeNegocio" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Código de cliente" for="CodigoCliente" runat="server" />
                    <asp:TextBox ID="CodigoCliente" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Nit" for="Nit" runat="server" />
                    <asp:TextBox ID="Nit" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Razón social" for="RazonSocial" runat="server" />
                    <asp:TextBox ID="RazonSocial" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Tipo de documento" for="TipoDocumento" runat="server" />
                    <asp:TextBox ID="TipoDocumento" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Número de docuemnto" for="NumeroDocumento" runat="server" />
                    <asp:TextBox ID="NumeroDocumento" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>


                 <div class="form-group col-md-4">
                    <asp:Label Text="Valor" for="Valor" runat="server" />
                    <asp:TextBox ID="Valor" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Saldo" for="Saldo" runat="server" />
                    <asp:TextBox ID="Saldo" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Fecha de inicio" for="FechaInicio" runat="server" />
                    <asp:TextBox ID="FechaInicio" CssClass="form-control" runat="server" TextMode="Date" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Fecha de fin" for="FechaFin" runat="server" />
                    <asp:TextBox ID="FechaFin" CssClass="form-control" runat="server" TextMode="Date" autocomplete="off"></asp:TextBox>
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
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered"  OnRowCommand="GridView1_RowCommand"  DataKeyNames="code" ShowHeaderWhenEmpty="true" >
                            <Columns>
                                <asp:ButtonField CommandName="cmdDetalle" ControlStyle-CssClass="btn btn-danger-unis btn-detalleUNIS" ButtonType="Button"  Text="Detalle" HeaderText="Detalle" />

                                <asp:TemplateField HeaderText="Código">
                                    <ItemTemplate>
                                        <div style="width: 200px;">
                                            <%# Eval("code")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Unidad de negocio">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("name_un")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Nit">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("nit")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código cliente">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("codecliente")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Razón social">
                                    <ItemTemplate>
                                        <div style="width: 250px;">
                                            <%# Eval("razonsocial")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Fecha">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                             <%# Eval("fecha","{0:yyyy-MM-dd}")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código tipo de documento">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("codtypedocument")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Tipo de documento">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("documenttype")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Número de documento">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("numdocument")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Días de crédito">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("diascredito")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
 
                                <asp:TemplateField HeaderText="Valor del documento">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("valordocument")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Saldo">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("saldo")%>
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
                            <h5 class="modal-title" id="exampleModalLabel">Detalle <span id="DocEncabezado"></span></h5>
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
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <h6>
                                                              <b>Nit:</b> <span id="NitCliente"></span>
                                                            </h6>
                                                            <h6>
                                                               <b>Razón social:</b> <span id="RazonSocialCliente"></span>
                                                            </h6>
                                                             <h6>
                                                               <b>Unidad de negocio:</b> <span id="UnidadNegocioCliente"></span>
                                                            </h6>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <h6>
                                                              <b>Días de crédito:</b> <span id="DiasCreditoDocumento"></span>
                                                            </h6>
                                                            <h6>
                                                               <b>Fecha:</b> <span id="FechaDocumento"></span>
                                                            </h6>
                                                             <h6>
                                                               <b>Tipo de documento:</b> <span id="TipoDeDocumento"></span>
                                                            </h6>
                                                             <h6>
                                                               <b>Número de documento:</b> <span id="NumeroDeDocumento"></span>
                                                            </h6>
                                                             <h6>
                                                               <b>Valor de documento:</b> <span id="ValorDeDocumento"></span>
                                                            </h6>
                                                            <h6>
                                                               <b>Saldo:</b> <span id="SaldoDocumento"></span>
                                                            </h6>
                                                        </div>
                                                       </div>
                                                            <br />
                                                        <div class="row">
                                                            <div class="col-md-12">
                                                                <h4>Detalle de cobro</h4>
                                                                <table class="table" id="tblDetalleCXC">
                                                                      <thead>
                                                                        <tr>
                                                                            <th scope="col">Código</th>
                                                                            <th scope="col">Unidad de negocio</th>
                                                                            <th scope="col">Número de documento</th>
                                                                            <th scope="col">Fecha</th>
                                                                            <th scope="col">Tipo de documento</th>
                                                                            <th scope="col">Número de documento de cobro</th>
                                                                            <th scope="col">Monto</th>
                                                                        </tr>
                                                                      </thead>
                                                                      <tbody>

                                                                      </tbody>
                                                                    </table>
                                                            </div>
                                            </div>
                                                </div>
                                             </div>
                                        </div>
                                    </form>           
                                </div>

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