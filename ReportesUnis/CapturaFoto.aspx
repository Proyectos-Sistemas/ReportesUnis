<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CapturaFoto.aspx.cs" Inherits="ReportesUnis.CapturaFoto" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2 style="text-align: center;">CARGA FOTO</h2>
        <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    </div>

    <div class="container2">
        <video id="videoElement" width="400" height="300" autoplay></video>
        <canvas id="canvas" width="400" height="300" ></canvas>
    </div>
    <button id="captureBtn" class="btn-danger-unis">Capturar imagen</button>
    <textarea id="urlPath" name="urlPath" style="display:none"></textarea>
    <button id="BtnAlmacenar" class="btn-danger-unis">Almacenar imagen</button>

    <script>
        // Acceder a la cámara y mostrar el video en el elemento de video
        navigator.mediaDevices.getUserMedia({ video: true })
            .then(function (stream) {
                var videoElement = document.getElementById('videoElement');
                videoElement.srcObject = stream;
            })
            .catch(function (error) {
                console.error('Error al acceder a la cámara: ', error);
            });

        // Capturar imagen cuando se haga clic en el botón
        var videoElement = document.getElementById('videoElement');
        var canvas = document.getElementById('canvas');
        var context = canvas.getContext('2d');
        var captureBtn = document.getElementById('captureBtn');
        const textarea = document.getElementById("urlPath");
        captureBtn.addEventListener('click', function () {
            context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);
            event.preventDefault();
            //Convertir la imagen del lienzo en base64
            var imageData = canvas.toDataURL('image/jpeg');
            textarea.value = imageData;
            
        });        
    </script>
</asp:Content>
