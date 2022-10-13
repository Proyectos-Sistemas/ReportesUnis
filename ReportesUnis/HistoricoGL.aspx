<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricoGL.aspx.cs" Inherits="ReportesUnis.HistoricoGL"  %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

        <br />
                    <h2 style="text-align:center;">
                   Históricos Contabilidad general</h2>
        <hr />
        <br />
        <div class="container">
            <div class="row">
                 <div class="form-group col-md-4">
                    <asp:Label Text="ID" for="Id" runat="server" />
                    <asp:TextBox ID="Id" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Unidad de Negocio" for="UnidadDeNegocio" runat="server" />
                    <asp:TextBox ID="UnidadDeNegocio" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Código cuenta contable" for="CodigoCuentaContable" runat="server" />
                    <asp:TextBox ID="CodigoCuentaContable" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>
                 <div class="form-group col-md-4">
                    <asp:Label Text="Período" for="Periodo" runat="server" />
                    <asp:TextBox ID="Periodo" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Descripción" for="Descripcion" runat="server" />
                    <asp:TextBox ID="Descripcion" CssClass="form-control" runat="server" autocomplete="off"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Fecha de inicio" for="FechaInicio" runat="server" />
                    <asp:TextBox ID="FechaInicio" CssClass="form-control" runat="server" TextMode="Date" autocomplete="off" required="true"></asp:TextBox>
                </div>

                 <div class="form-group col-md-4">
                    <asp:Label Text="Fecha de fin" for="FechaFin" runat="server" />
                    <asp:TextBox ID="FechaFin" CssClass="form-control" runat="server" TextMode="Date" autocomplete="off"  required="true"></asp:TextBox>
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
                        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" CssClass="table table-condensed table-bordered"  ShowHeaderWhenEmpty="true" >
                            <Columns>
                               
                                <asp:TemplateField HeaderText="Correlativo transacción">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("id")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código">
                                    <ItemTemplate>
                                        <div style="width: 250px;">
                                            <%# Eval("code")%>
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

                                <asp:TemplateField HeaderText="Código Unidad de negocio">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("unidadnegocio_code")%>
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

                                <asp:TemplateField HeaderText="Código centro de integración">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("centrointegracion_code")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Centro de integración">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("name_ci")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Código cuenta contable">
                                    <ItemTemplate>
                                        <div style="width: 175px;">
                                            <%# Eval("cuentacontable_code")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>



                                <asp:TemplateField HeaderText="Cuenta contable">
                                    <ItemTemplate>
                                        <div style="width: 300px;">
                                            <%# Eval("name_cc")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Debe">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("debe")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Haber">
                                    <ItemTemplate>
                                        <div style="width: 150px;">
                                            <%# Eval("haber")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Período">
                                    <ItemTemplate>
                                        <div style="width: 125px;">
                                            <%# Eval("periodo")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
 
                                <asp:TemplateField HeaderText="Descripción">
                                    <ItemTemplate>
                                        <div style="width: 250px;">
                                            <%# Eval("descripcion")%>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>                          
                            </Columns>                                
                        </asp:GridView>
                    </div>           
            </div>
        </div>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>
</asp:Content>