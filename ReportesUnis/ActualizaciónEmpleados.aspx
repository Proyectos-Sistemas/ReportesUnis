<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ActualizaciónEmpleados.aspx.cs" Inherits="ReportesUnis.ActualizaciónEmpleados" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <br />
    <div id="CargaFotografia" runat="server" visible="true">
        <div class="container">
            <div class="row">
                <div class="form-group col">
                    <h2 style="text-align: center;">ACTUALIZACIÓN DE INFORMACIÓN</h2>
                </div>
            </div>
        </div>
        <hr />
        <div class="container">
            <div class="row">
                <div class="form-group  col-md">
                    <asp:HiddenField runat="server" ID="hdnCameraAvailable" />
                    <h5 style="text-align: center;">Toma de Fografía</h5>
                </div>
            </div>
        </div>
        <br />

        <div class="container">
            <div class="row">
                <div class="form-group col-md-1">
                </div>

                <div class="form-group col-md-4">
                    <video id="videoElement" width="350" height="250" autoplay></video>
                </div>
                <div class="form-group col-md-2">
                    <asp:Label ID="Label1" runat="server" Visible="true" ForeColor="White"> </asp:Label>
                </div>

                <div class="form-group col-md-4">
                    <asp:Image ID="ImgBase" runat="server" Width="350" Height="250" Visible="true" />
                </div>

                <div class="form-group col-md-1">
                    <canvas id="canvas" width="350" height="250" style="display: none"></canvas>
                </div>
            </div>
        </div>

        <textarea id="urlPath" name="urlPath" style="display: none"></textarea>
        <div class="container">
            <div class="row">
                <div class="col-md-4">
                </div>

                <div class="col-md-4 mx-auto text-center d-flex align-items-center justify-content-center">
                    <button id="captureBtn" name="captureBtn" class="btn-danger-unis">Capturar imagen</button>
                </div>

                <div class="col-md-4">
                </div>
            </div>
        </div>
        <br />
        <hr />

        <div class="container" id="CargaDPI" runat="server" style="display: none">
            <div>
                <h5 style="text-align: center;">Carga de Documento de identificación</h5>
            </div>
            <asp:Label ID="Label3" runat="server" Font-Bold="false">Para realizar un cambio en su nombre es necesario adjuntar según sea el caso:</asp:Label>
            <br />
            <asp:Label ID="Label4" runat="server" Font-Bold="false" Font-Size="Small">a.) Fotografia de su DPI de ambos lados, es decir 2 fotografías</asp:Label>
            <br />
            <asp:Label ID="Label5" runat="server" Font-Bold="false" Font-Size="Small">b.) Fotografia de su Pasaporte</asp:Label>
            <br />
            <br />
            <asp:FileUpload ID="FileUpload2" runat="server" AllowMultiple="true" accept="image/jpeg" onchange="validarCargaArchivos();" />
            <div id="dvMsge" style="background-color: Red; color: White; width: 190px; padding: 3px; display: none;">
                El tamaño máximo permitido es de 1 GB
            </div>
            <br />
            <hr />
        </div>

        <div id="CamposAuxiliares" runat="server" visible="true">
            <%-- TXTEXISTE2 ALMACENA vALORES PARA HACER VALIDACIONES --%>
            <asp:Label ID="txtExiste2" runat="server" Visible="false"></asp:Label>
            <%-- NOMBRE INICIAL--%>
            <%-- TXTURL SE UTILIZA PARA ALMACENAR LA URL PARA LA CONSULTA DEL WS --%>
            <asp:TextBox ID="TxtURL" runat="server" Visible="false"></asp:TextBox>
            <%-- TXTUSER ALMACENA EL DPI DEL USUARIO QUE ESTA REALIZANDO CAMBIOS --%>
            <asp:TextBox ID="TextUser" runat="server" Visible="false"></asp:TextBox>
            <%-- TEXTBOX USEREMPLID ALMACENA EL EMPLID DEL USUARIO QUE ESTA HACIENDO LA ACTUALIZACION --%>
            <asp:TextBox ID="UserEmplid" runat="server" Visible="false"></asp:TextBox>
            <%-- TEXTBOX ALMACENA EL STATE AL MOMENTO DE SELECCIONAR EL MUNICIPIO--%>
            <asp:TextBox ID="State" runat="server" Visible="false"></asp:TextBox>
            <asp:TextBox ID="Pais" runat="server" Visible="false"></asp:TextBox>
            <asp:TextBox ID="PaisInicial" runat="server" Visible="false"></asp:TextBox>
            <asp:TextBox ID="Txtsustituto" runat="server" Visible="false"></asp:TextBox>
            <%-- TXTINSERT ALMACENA EL QUERY PARA HACER INSERT EN EL BANCO --%>
            <asp:TextBox ID="txtInsertBI" runat="server" Visible="false"></asp:TextBox>
        </div>

        <div id="InfePersonal" runat="server">
            <div class="container">
                <div class="row">
                    <div class="form-group  col-md">
                        <h5 id="HPersonal" style="text-align: center;">Información Personal</h5>
                    </div>
                </div>
            </div>
            <%-- NOMBRE 1 INICIAL--%>
            <asp:Label ID="txtNInicial1" runat="server" Visible="true" ForeColor="White"></asp:Label>
            <%-- APELLIDO 1 INICIAL --%>
            <asp:Label ID="txtAInicial1" runat="server" Visible="true" ForeColor="White"></asp:Label>
            <%-- NOMBRE 2 INICIAL--%>
            <asp:Label ID="txtNInicial2" runat="server" Visible="true" ForeColor="White"></asp:Label>
            <%-- APELLIDO 2 INICIAL --%>
            <asp:Label ID="txtAInicial2" runat="server" Visible="true" ForeColor="White"></asp:Label>
            <%-- APELLIDO CASADA INICIAL --%>
            <asp:Label ID="txtCInicial" runat="server" Visible="true" ForeColor="White"></asp:Label>

            <%-- TABLA EN LA QUE SE COLOCAN LOS OBJETOS --%>
            <div class="container" id="tabla" runat="server">
                <div class="row">
                    <div class="col-md">
                        <div class="container">
                            <div class="row">

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">DPI/Pasaporte:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtdPI" runat="server" Enabled="false"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Fecha de Nacimiento:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtCumple" runat="server" Enabled="false"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Facultad o Dependencia:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtFacultad" runat="server" Enabled="false" TextMode="MultiLine" Rows="2"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Puesto:</asp:Label>
                                    <br />
                                    <asp:Label ID="txtPuesto" runat="server" Enabled="true" TextMode="MultiLine" Rows="2"></asp:Label>
                                </div>

                                <div class="form-group col-md-4">
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Primer Nombre:</asp:Label>
                                    <asp:TextBox ID="txtNombre1" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtNombre1" ErrorMessage="Ingrese su nombre." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Segundo Nombre:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtNombre2" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="form-group col-md-4">
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Primer Apellido:</asp:Label>
                                    <asp:TextBox ID="txtApellido1" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtApellido1" ErrorMessage="Ingrese su apellido." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Segundo Apellido:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtApellido2" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Apellido de Casada:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtApellidoCasada" runat="server" Enabled="true" MaxLength="30" Width="275px" CssClass="form-control"></asp:TextBox>
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Dirección*:</asp:Label>
                                    <asp:TextBox ID="txtDireccion" runat="server" TextMode="MultiLine" Rows="2" MaxLength="240" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <br />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDireccion" ErrorMessage="Ingrese su dirección." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Dirección2:</asp:Label>
                                    <br />
                                    <asp:TextBox ID="txtDireccion2" runat="server" TextMode="MultiLine" Rows="2" MaxLength="240" Width="275px" CssClass="form-control"></asp:TextBox>
                                </div>

                                <div class="form-group col-md-4">
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">País*:</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="cMBpAIS" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cMBpAIS_SelectedIndexChanged" EnableViewState="true" Width="275px" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Departamento*:</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="CmbDepartamento" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbDepartamento_SelectedIndexChanged" Width="275px" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="form-group col-md-4">
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Municipio*:</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="CmbMunicipio" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="CmbMunicipio_SelectedIndexChanged" Width="275px" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Zona:</asp:Label>
                                    <br />
                                    <asp:DropDownList ID="txtZona" runat="server" AutoPostBack="true" EnableViewState="true" Width="275px" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>

                                <div class="form-group col-md-4">
                                </div>



                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Estado Civil:</asp:Label>
                                    <br />
                                    <br />
                                    <asp:DropDownList ID="CmbEstado" runat="server" Width="275px" CssClass="form-control">
                                        <asp:ListItem Selected="False" Value=""></asp:ListItem>
                                        <asp:ListItem>Casado</asp:ListItem>
                                        <asp:ListItem>Soltero</asp:ListItem>
                                        <asp:ListItem>Sin Información</asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                </div>

                                <div class="form-group col-md-4">
                                    <asp:Label runat="server" Font-Bold="true">Teléfono:</asp:Label>
                                    <asp:CustomValidator ID="validarTelefono" runat="server" ControlToValidate="txtTelefono" ErrorMessage="El número de teléfono debe de tener al menos 8 caracteres" Font-Size="Small" ClientValidationFunction="VerificarCantidadTelefono" ForeColor="Red"></asp:CustomValidator>
                                    <asp:TextBox ID="txtTelefono" runat="server" MaxLength="24" Width="275px" CssClass="form-control"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtTelefono" ErrorMessage="Ingrese un número de teléfono." ForeColor="Red"> </asp:RequiredFieldValidator>
                                </div>

                                <div class="form-group col-md-4">
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:Table ID="tbactualizar" runat="server" Style="margin-left: auto; margin-right: auto; text-align: center; align-content: center">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Button ID="BtnActualizar" runat="server" Text="Actualizar" CssClass="btn-danger-unis" Enabled="true" OnClientClick="return mostrarAlerta();" OnClick="BtnActualizar_Click" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br />
        <div style="margin-left: auto; margin-right: auto; text-align: center;">
            <asp:Label ID="lblActualizacion" runat="server" Font-Bold="true" ForeColor="Red" Text="" Font-Size="Large"> 
            </asp:Label>
            <br />
            <%--<asp:Button ID="BtnDownload" runat="server" Text="Descargar Manual" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnDownload_Click" Visible="false" />
            <asp:Button ID="BtnReload" runat="server" Text="Recargar Página" CssClass="btn-danger-unis" Enabled="true" OnClick="BtnReload_Click" Visible="false" />--%>
            <br />
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">
        function VerificarCantidadTelefono(sender, args) {
            args.IsValid = (args.Value.length >= 7);
        }


        // Acceder a la cámara y mostrar el video en el elemento de video
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                var videoElement = document.getElementById('videoElement');
                videoElement.srcObject = stream;
            })
            .catch(function (error) {
                console.error('Error al acceder a la cámara: ', error);
            });

        $(document).ready(function () {
            var videoElement = $('#videoElement')[0];
            var canvas = $('#canvas')[0];
            var context = canvas.getContext('2d');
            var captureBtn = $('#captureBtn');
            var textarea = $("#urlPath");
            var imgBase = $("#<%= ImgBase.ClientID %>");
            captureBtn.on('click', function (event) {
                event.preventDefault();
                context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
                var imageData = canvas.toDataURL('image/jpeg');
                textarea.val(imageData);
                imgBase.attr('src', imageData);
                canvas.hide();
            });
        });

        $(document).ready(function () {
            $('#<%= txtNombre1.ClientID %> , #<%= txtNombre2.ClientID %> , #<%= txtApellido1.ClientID %>, #<%= txtApellido2.ClientID %>, #<%= txtApellidoCasada.ClientID %>').on('input', function () {
                var txtNombre1 = $('#<%= txtNombre1.ClientID %>').val().trim();
                var txtNInicial1 = $('#<%= txtNInicial1.ClientID %>').text().trim();
                var txtNombre2 = $('#<%= txtNombre2.ClientID %>').val().trim();
                var txtNInicial2 = $('#<%= txtNInicial2.ClientID %>').text().trim();
                var txtApellido1 = $('#<%= txtApellido1.ClientID %>').val().trim();
                var txtAInicial1 = $('#<%= txtAInicial1.ClientID %>').text().trim();
                var txtApellido2 = $('#<%= txtApellido2.ClientID %>').val().trim();
                var txtAInicial2 = $('#<%= txtAInicial2.ClientID %>').text().trim();
                var txtApellidoCasada = $('#<%= txtApellidoCasada.ClientID %>').val().trim();
                var txtCInicial = $('#<%= txtCInicial.ClientID %>').text().trim();

                if (txtNombre1 !== txtNInicial1 || txtNombre2 !== txtNInicial2 || txtApellido1 !== txtAInicial1 || txtApellido2 !== txtAInicial2 || txtApellidoCasada !== txtCInicial) {
                    $('#<%= CargaDPI.ClientID %>').css('display', 'block');
                } else {
                    $('#<%= CargaDPI.ClientID %>').css('display', 'none');
                }
            });
        });

        function checkCameraAccess() {
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(function (stream) {
                    document.getElementById('<%= hdnCameraAvailable.ClientID %>').value = 'true';
                    stream.getTracks().forEach(function (track) {
                        track.stop();
                    });
                })
                .catch(function (error) {
                    document.getElementById('<%= hdnCameraAvailable.ClientID %>').value = 'false';
                });
        }

        function validarCargaArchivos() {
            var fileUpload = document.getElementById('<%= FileUpload2.ClientID %>');
            var files = fileUpload.files;

            if (files.length > 2) {
                alert("Solo se permiten cargar 2 archivos.");
                // Eliminar los archivos adicionales
                while (files.length > 2) {
                    fileUpload.remove(files.length - 1);
                }
            }
        }
        //Validar Numeros de telefono
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

        function mostrarAlerta() {
            var mensaje = "";
            var apellido = document.getElementById('<%= txtApellido1.ClientID %>').value;
            var nombre = document.getElementById('<%= txtNombre1.ClientID %>').value;
            var direccion1 = document.getElementById('<%= txtDireccion.ClientID %>').value;
            var telefono = document.getElementById('<%= txtTelefono.ClientID %>').value;
            var pais = document.getElementById('<%= cMBpAIS.ClientID %>').value;
            var depto = document.getElementById('<%= CmbDepartamento.ClientID %>').value;
            var muni = document.getElementById('<%= CmbMunicipio.ClientID %>').value;
            var foto = document.getElementById('urlPath').value;

            if (apellido.trim() === "") {
                mensaje = "-Los Apellidos son requerido.";
            }

            if (nombre.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-El primer nombre es requerido.";
                } else {
                    mensaje = mensaje + "\n-El primer nombres es requerido.";
                }
            }

            if (direccion1.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-La dirección es requerida.";
                } else {
                    mensaje = mensaje + "\n-La dirección es requerida.";
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

            if (telefono.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-El teléfono es requerido.";
                } else {
                    mensaje = mensaje + "\n-El teléfono es requerido.";
                }
            }

            if (telefono.length > 0 && telefono.length <= 7) {
                if (mensaje.trim() == "") {
                    mensaje = "-El teléfono debe de tener 8 carácteres";
                } else {
                    mensaje = mensaje + "\n-El teléfono debe de tener 8 carácteres";
                }
            }

            if (foto.trim() === "") {
                if (mensaje.trim() == "") {
                    mensaje = "-La fotografía es requerida";
                } else {
                    mensaje = mensaje + "\n-La fotografía es requerida";
                }
            }

            if (mensaje.trim() !== "") {
                mensaje = mensaje.replace("/\n/g", "<br>");
                alert(mensaje);
                return false;
            } else if (confirm("¿Está seguro de que su información es correcta?")) {
                __doPostBack('<%= BtnActualizar.ClientID %>', '');
                return true; // Permite continuar con la acción del botón
            } else {
                return false; // Cancela la acción del botón
            }
        }

        function Documentos() {
            alert("Es necesario adjuntar la imagen de su documento de actualización para continuar con la actualización.");
        }

        function ConfirmacionActualizacionSensible() {
            mensaje = "Su información fue almacenada correctamente. \nLa información ingresada debe ser aprobada antes de ser confirmada.\nActualmente, solo se muestran los datos que han sido previamente confirmados.";
            mensaje = mensaje.replace("/\n/g", "<br>");
            alert(mensaje);
            window.location.href = "ActualizacionEstudiantes.aspx";
        }
        function ConfirmacionActualizacion() {
            mensaje = "Su información fue actualizada correctamente.";
            mensaje = mensaje.replace("/\n/g", "<br>");
            alert(mensaje);
            window.location.href = "ActualizacionEstudiantes.aspx";
        }


    </script>
    <script src="Scripts/UNIS/Unis.js"></script>
    <div class="preloader" id="preloader"></div>

</asp:Content>
