﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReporteCamarasTermicas.aspx.cs" Inherits="ReportesUnis.ReporteCamarasTermicas" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">REPORTE CÁMARAS TÉRMICAS ESTUDIANTES</h2>
    </div>
    <div class="container">
        <hr />
        <div>
            <a>Busqueda por: </a>
            <%-- DROPDOWLIST CON OPCIONES DE BUSQUEDA --%>
            <asp:DropDownList ID="LbxBusqueda" SelectionMode="Single" runat="server">
                <asp:ListItem Selected="True" Value=""></asp:ListItem>
                <asp:ListItem>Nombre</asp:ListItem>
                <asp:ListItem>Apellido</asp:ListItem>
                <asp:ListItem>ID</asp:ListItem>
                <asp:ListItem>Género</asp:ListItem>
                <asp:ListItem>Departamento</asp:ListItem>
            </asp:DropDownList>

             <%-- CALENDARIO PARA BUSQUEDA --%>
            <asp:Label ID="FInicio" runat="server">Fecha inicio:</asp:Label>
            <asp:TextBox ID="CldrCiclosInicio" runat="server" TextMode="Date"> </asp:TextBox>
            <asp:Label ID="FFin" runat="server">Fecha fin:</asp:Label>
            <asp:TextBox ID="CldrCiclosFin" runat="server" TextMode="Date">
            </asp:TextBox>

            <%-- TXTBUSCADOR DONDE SE INGRESA EL TEXTO A BUSCAR --%>
            <asp:TextBox ID="TxtBuscador" runat="server"></asp:TextBox>

            <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
            <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>
            <asp:Button ID="BtnBuscar2" runat="server" Text="Buscar" OnClick="Busqueda" CssClass="btn-danger-unis"></asp:Button>
            <asp:Button ID="BtnTxt" runat="server" Text="Exportar Excel" CssClass="btn-danger-unis" OnClick="GenerarExcel" Enabled="true" />
            <asp:Button ID="BtnImg" runat="server" Text="Exportar Imagenes" CssClass="btn-danger-unis" OnClick="BtnImg_Click" Enabled="true" />
        </div>
        <br />

        <div style="text-align: center">
            <br />
            <asp:Label ID="lblBusqueda" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"></asp:Label>
            <asp:Label ID="lblDescarga" runat="server" Font-Bold="true" ForeColor="Blue" Text="" Font-Size="Large" Visible="false"></asp:Label>
        </div>       
    </div>

    <div class="container-fluid">
        <div class="row">
            <div class="col-md-12">
                 <%-- GRIDVIEW DONDE SE MUESTRA LA INFORMACION DEL REPORTE --%>
        <div class="containerGV" id="GVContainer">
            <asp:GridView ID="GridViewReporteCT" runat="server"
                AutoGenerateColumns="false" CssClass="table table-condensed table-bordered ">
                <Columns>
                    <asp:BoundField DataField="FIRST_NAME" HeaderText="First Name" />
                    <asp:BoundField DataField="LAST_NAME" HeaderText="Last Name" />
                    <asp:BoundField DataField="ID" HeaderText="ID" />
                    <asp:BoundField DataField="TYPE" HeaderText="Type" />
                    <asp:BoundField DataField="PERSON_GROUP" HeaderText="Person Group" />
                    <asp:BoundField DataField="GENDER" HeaderText="Gender" />
                    <asp:BoundField DataField="Start_Time_of_Effective_Period" HeaderText="Start Time of Effective Period" />
                    <asp:BoundField DataField="End_Time_of_Effective_Period" HeaderText="End Time of Effective Period" />
                    <asp:BoundField DataField="CARD" HeaderText="Card" />
                    <asp:BoundField DataField="EMAIL" HeaderText="Email" />
                    <asp:BoundField DataField="PHONE" HeaderText="Phone" />
                    <asp:BoundField DataField="REMARK" HeaderText="Remark" />
                    <asp:BoundField DataField="DOCK_STATION_LOGIN_PASSWORD" HeaderText="Dock Station Login Password" />
                    <asp:BoundField DataField="SUPPORTISSUEDCUSTOMPROPERTIES" HeaderText="Support Issued Custom Properties" />
                    <asp:BoundField DataField="SKINSURFACE_TEMPERATURE" HeaderText="Skin-surface Temperature" />
                    <asp:BoundField DataField="TEMPERATURE_STATUS" HeaderText="Temperature Status" />
                    <asp:BoundField DataField="DEPARTAMENTO" HeaderText="Departamento" />
                    <asp:BoundField DataField="EMPLID" HeaderText="Emplid" Visible="false"/>
                </Columns>
            </asp:GridView>
        </div>
            </div>
        </div>
    </div>

    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>

</asp:Content>
