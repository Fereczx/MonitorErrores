const chatButton = document.getElementById("chat-button");
const chatWindow = document.getElementById("chat-window");

const minimizeButton = document.getElementById("minimize-chat");
const closeButton = document.getElementById("close-chat");

const attachButton = document.getElementById("attach-button");
const imageInput = document.getElementById("image-input");

const messageInput = document.getElementById("message-input");
const sendButton = document.getElementById("send-button");

const chatMessages = document.getElementById("chat-messages");

const imagePreview = document.getElementById("image-preview");
const previewImage = document.getElementById("preview-image");
const imageName = document.getElementById("image-name");

const removeImageButton =
    document.getElementById("remove-image");

const analyzeImageButton =
    document.getElementById("analyze-image");

const imageModal =
    document.getElementById("image-modal");

const modalImage =
    document.getElementById("modal-image");

const closeImageModalButton =
    document.getElementById("close-image-modal");


// =========================
// CONFIGURACIÓN
// =========================

const API_URL =
    "http://localhost:5163/api/Errores";

const API_IMAGE_URL =
    "http://localhost:5163/api/Errores/imagen";


// Imagen actualmente seleccionada
let imagenSeleccionada = null;


// URL temporal utilizada para la vista previa
let imagenPreviewUrl = null;


// =========================
// ABRIR CHAT
// =========================

chatButton.addEventListener("click", () => {

    chatWindow.style.display = "flex";

    chatButton.style.display = "none";

    messageInput.focus();
});


// =========================
// CERRAR CHAT
// =========================

closeButton.addEventListener("click", () => {

    chatWindow.style.display = "none";

    chatButton.style.display = "flex";
});


// =========================
// MINIMIZAR CHAT
// =========================

minimizeButton.addEventListener("click", () => {

    chatWindow.style.display = "none";

    chatButton.style.display = "flex";
});


// =========================
// ABRIR SELECTOR DE IMAGEN
// =========================

attachButton.addEventListener("click", () => {

    imageInput.click();
});


// =========================
// IMAGEN SELECCIONADA
// =========================

imageInput.addEventListener("change", () => {

    if (imageInput.files.length === 0) {
        return;
    }

    const archivo = imageInput.files[0];

    mostrarVistaPrevia(archivo);
});


// =========================
// MOSTRAR VISTA PREVIA
// =========================

function mostrarVistaPrevia(archivo) {

    imagenSeleccionada = archivo;

    // Liberar una URL anterior
    if (imagenPreviewUrl) {

        URL.revokeObjectURL(
            imagenPreviewUrl
        );
    }

    // Crear URL temporal
    imagenPreviewUrl =
        URL.createObjectURL(archivo);

    // Mostrar imagen
    previewImage.src =
        imagenPreviewUrl;

    // Mostrar nombre
    imageName.textContent =
        archivo.name;

    // Mostrar contenedor
    imagePreview.classList.add("active");
}


// =========================
// QUITAR IMAGEN
// =========================

removeImageButton.addEventListener("click", () => {

    limpiarVistaPrevia();
});


// =========================
// LIMPIAR VISTA PREVIA
// =========================

function limpiarVistaPrevia() {

    imagenSeleccionada = null;

    if (imagenPreviewUrl) {

        URL.revokeObjectURL(
            imagenPreviewUrl
        );

        imagenPreviewUrl = null;
    }

    previewImage.src = "";

    imageName.textContent = "";

    imagePreview.classList.remove(
        "active"
    );

    imageInput.value = "";
}


// =========================
// ANALIZAR IMAGEN
// =========================

analyzeImageButton.addEventListener(
    "click",
    analizarImagen
);


async function analizarImagen() {

    if (!imagenSeleccionada) {
        return;
    }

    const archivo = imagenSeleccionada;

    // Ocultar la vista previa
    limpiarVistaPrevia();

    // Mostrar imagen dentro del chat
    agregarImagenUsuario(archivo);

    setEstadoEnvio(false);

    // Mostrar indicador de análisis
    const mensajeCarga =
        agregarMensajeAnalizando();

    try {

        const formData =
            new FormData();

        formData.append(
            "Imagen",
            archivo
        );

        console.log(
            "Enviando imagen al backend:",
            archivo.name
        );

        const respuesta =
            await fetch(
                API_IMAGE_URL,
                {
                    method: "POST",
                    body: formData
                }
            );

        if (!respuesta.ok) {

            throw new Error(
                `Error HTTP ${respuesta.status}`
            );
        }

        const resultado =
            await respuesta.json();

        console.log(
            "Respuesta del análisis de imagen:",
            resultado
        );

        // Quitar indicador
        mensajeCarga.remove();

        // Mostrar resultado
        mostrarResultado(resultado);

    }
    catch (error) {

        console.error(
            "Error analizando la imagen:",
            error
        );

        // Quitar indicador
        mensajeCarga.remove();

        agregarMensajeBot(
            "No pudimos analizar la imagen. Por favor, intentá nuevamente."
        );
    }
    finally {

        setEstadoEnvio(true);

        messageInput.focus();
    }
}


// =========================
// ENVIAR MENSAJE
// =========================

sendButton.addEventListener(
    "click",
    enviarMensaje
);


// =========================
// ENTER PARA ENVIAR
// =========================

messageInput.addEventListener(
    "keydown",
    (event) => {

        if (
            event.key === "Enter" &&
            !event.shiftKey
        ) {

            event.preventDefault();

            enviarMensaje();
        }
    }
);


// =========================
// ENVIAR MENSAJE A LA API
// =========================

async function enviarMensaje() {

    const mensaje =
        messageInput.value.trim();

    if (mensaje === "") {
        return;
    }

    agregarMensajeUsuario(
        mensaje
    );

    messageInput.value = "";

    setEstadoEnvio(false);

    // Mostrar indicador de análisis
    const mensajeCarga =
        agregarMensajeAnalizando();

    try {

        const error = {

            codigo:
                detectarCodigo(
                    mensaje
                ),

            servicio:
                detectarServicio(
                    mensaje
                ),

            mensaje:
                mensaje
        };

        console.log(
            "Enviando error al backend:",
            error
        );

        const respuesta =
            await fetch(
                API_URL,
                {
                    method: "POST",

                    headers: {
                        "Content-Type":
                            "application/json"
                    },

                    body:
                        JSON.stringify(
                            error
                        )
                }
            );

        if (!respuesta.ok) {

            throw new Error(
                `Error HTTP ${respuesta.status}`
            );
        }

        const resultado =
            await respuesta.json();

        console.log(
            "Respuesta del backend:",
            resultado
        );

        // Quitar indicador
        mensajeCarga.remove();

        // Mostrar resultado
        mostrarResultado(
            resultado
        );

    }
    catch (error) {

        console.error(
            "Error comunicándose con el backend:",
            error
        );

        // Quitar indicador
        mensajeCarga.remove();

        agregarMensajeBot(
            "No pudimos comunicarnos con el sistema. Por favor, intentá nuevamente."
        );
    }
    finally {

        setEstadoEnvio(true);

        messageInput.focus();
    }
}


// =========================
// MOSTRAR RESULTADO
// =========================

function mostrarResultado(resultado) {

    if (!resultado) {

        agregarMensajeBot(
            "No recibimos una respuesta válida del sistema."
        );

        return;
    }


    // =========================
    // SOLUCIÓN COMPLETA
    // =========================

    const solucionCompleta =
        resultado.solucionCompleta === true ||
        (
            resultado.origen === "conocimiento" &&
            resultado.confirmada === true
        );


    if (solucionCompleta) {

        const contenedor =
            crearContenedorMensajeBot();

        contenedor.mensaje.classList.add(
            "solution-message"
        );


        // Problema detectado
        if (resultado.problema) {

            const seccionProblema =
                document.createElement("div");

            seccionProblema.classList.add(
                "result-section"
            );


            const titulo =
                document.createElement("span");

            titulo.classList.add(
                "result-title"
            );

            titulo.textContent =
                "Problema detectado";


            const texto =
                document.createElement("div");

            texto.textContent =
                resultado.problema;


            seccionProblema.appendChild(
                titulo
            );

            seccionProblema.appendChild(
                texto
            );

            contenedor.mensaje.appendChild(
                seccionProblema
            );
        }


        // Solución / mensaje para el usuario
        const mensajeSolucion =
            resultado.mensajeUsuario ||
            resultado.solucion;


        if (mensajeSolucion) {

            const seccionSolucion =
                document.createElement("div");

            seccionSolucion.classList.add(
                "result-section"
            );


            const titulo =
                document.createElement("span");

            titulo.classList.add(
                "result-title"
            );

            titulo.textContent =
                "Solución";


            const texto =
                document.createElement("div");

            texto.textContent =
                mensajeSolucion;


            seccionSolucion.appendChild(
                titulo
            );

            seccionSolucion.appendChild(
                texto
            );

            contenedor.mensaje.appendChild(
                seccionSolucion
            );
        }


        chatMessages.appendChild(
            contenedor.elemento
        );

        desplazarChatAlFinal();

        return;
    }


    // =========================
    // ATENCIÓN AL CLIENTE
    // =========================

    if (resultado.mensajeUsuario) {

        const contenedor =
            crearContenedorMensajeBot();


        contenedor.mensaje.classList.add(
            "customer-service-message"
        );


        const titulo =
            document.createElement("span");

        titulo.classList.add(
            "result-title"
        );

        titulo.textContent =
            "Atención al Cliente";


        const texto =
            document.createElement("div");

        texto.textContent =
            resultado.mensajeUsuario;


        contenedor.mensaje.appendChild(
            titulo
        );

        contenedor.mensaje.appendChild(
            texto
        );


        chatMessages.appendChild(
            contenedor.elemento
        );

        desplazarChatAlFinal();

        return;
    }


    // =========================
    // RESPUESTA POR DEFECTO
    // =========================

    agregarMensajeBot(
        "No pudimos determinar una solución para este problema. Por favor, comunicate con Atención al Cliente para recibir asistencia."
    );
}


// =========================
// CREAR CONTENEDOR DE MENSAJE BOT
// =========================

function crearContenedorMensajeBot() {

    // Contenedor principal
    const elemento =
        document.createElement(
            "div"
        );

    elemento.classList.add(
        "bot-message-container"
    );


    // Avatar
    const avatar =
        document.createElement(
            "div"
        );

    avatar.classList.add(
        "bot-avatar"
    );

    avatar.textContent =
        "M";


    // Caja del mensaje
    const mensaje =
        document.createElement(
            "div"
        );

    mensaje.classList.add(
        "message",
        "bot-message"
    );


    // Construir estructura
    elemento.appendChild(
        avatar
    );

    elemento.appendChild(
        mensaje
    );


    return {
        elemento,
        mensaje
    };
}


// =========================
// AGREGAR MENSAJE USUARIO
// =========================

function agregarMensajeUsuario(
    mensaje
) {

    const elemento =
        document.createElement(
            "div"
        );

    elemento.classList.add(
        "message",
        "user-message"
    );

    elemento.textContent =
        mensaje;

    chatMessages.appendChild(
        elemento
    );

    desplazarChatAlFinal();
}


// =========================
// AGREGAR IMAGEN DEL USUARIO
// =========================

function agregarImagenUsuario(archivo) {

    const elemento =
        document.createElement("div");

    elemento.classList.add(
        "message",
        "user-message"
    );


    const imagen =
        document.createElement("img");

    imagen.classList.add(
        "chat-image"
    );


    const url =
        URL.createObjectURL(archivo);

    imagen.src =
        url;

    imagen.alt =
        "Captura enviada";


    // Al hacer clic, ampliar la imagen
    imagen.addEventListener(
        "click",
        () => {

            if (
                !imageModal ||
                !modalImage
            ) {
                return;
            }

            modalImage.src =
                url;

            imageModal.classList.add(
                "active"
            );
        }
    );


    elemento.appendChild(
        imagen
    );

    chatMessages.appendChild(
        elemento
    );

    desplazarChatAlFinal();
}


// =========================
// AGREGAR MENSAJE BOT
// =========================

function agregarMensajeBot(
    mensaje
) {

    // Contenedor del mensaje
    const contenedor =
        document.createElement(
            "div"
        );

    contenedor.classList.add(
        "bot-message-container"
    );


    // Avatar
    const avatar =
        document.createElement(
            "div"
        );

    avatar.classList.add(
        "bot-avatar"
    );

    avatar.textContent = "M";


    // Mensaje
    const elemento =
        document.createElement(
            "div"
        );

    elemento.classList.add(
        "message",
        "bot-message"
    );

    elemento.textContent =
        mensaje;


    // Construir estructura
    contenedor.appendChild(
        avatar
    );

    contenedor.appendChild(
        elemento
    );

    chatMessages.appendChild(
        contenedor
    );

    desplazarChatAlFinal();

    return contenedor;
}


// =========================
// INDICADOR DE ANÁLISIS
// =========================

function agregarMensajeAnalizando() {

    // Contenedor
    const contenedor =
        document.createElement(
            "div"
        );

    contenedor.classList.add(
        "bot-message-container"
    );


    // Avatar
    const avatar =
        document.createElement(
            "div"
        );

    avatar.classList.add(
        "bot-avatar"
    );

    avatar.textContent = "M";


    // Mensaje
    const mensaje =
        document.createElement(
            "div"
        );

    mensaje.classList.add(
        "message",
        "bot-message",
        "typing-message"
    );


    // Primer punto
    const punto1 =
        document.createElement(
            "span"
        );

    punto1.classList.add(
        "typing-dot"
    );


    // Segundo punto
    const punto2 =
        document.createElement(
            "span"
        );

    punto2.classList.add(
        "typing-dot"
    );


    // Tercer punto
    const punto3 =
        document.createElement(
            "span"
        );

    punto3.classList.add(
        "typing-dot"
    );


    // Construir indicador
    mensaje.appendChild(
        punto1
    );

    mensaje.appendChild(
        punto2
    );

    mensaje.appendChild(
        punto3
    );


    // Construir contenedor
    contenedor.appendChild(
        avatar
    );

    contenedor.appendChild(
        mensaje
    );

    chatMessages.appendChild(
        contenedor
    );

    desplazarChatAlFinal();

    return contenedor;
}


// =========================
// DESPLAZAR CHAT
// =========================

function desplazarChatAlFinal() {

    chatMessages.scrollTop =
        chatMessages.scrollHeight;
}


// =========================
// ACTIVAR / DESACTIVAR ENVÍO
// =========================

function setEstadoEnvio(
    activado
) {

    sendButton.disabled =
        !activado;

    messageInput.disabled =
        !activado;

    attachButton.disabled =
        !activado;

    analyzeImageButton.disabled =
        !activado;
}


// =========================
// DETECTAR CÓDIGO
// =========================

function detectarCodigo(
    mensaje
) {

    const texto =
        mensaje.toUpperCase();


    const coincidenciaHttp =
        texto.match(
            /\b(400|401|403|404|405|408|409|415|422|429|500|502|503|504)\b/
        );


    if (coincidenciaHttp) {

        return coincidenciaHttp[1];
    }


    const codigosConocidos = [

        "TIMEOUT",

        "PAGO_RECHAZADO",

        "PAGO_DUPLICADO",

        "PAGO_PENDIENTE",

        "SALDO_INSUFICIENTE",

        "TARJETA_RECHAZADA",

        "TARJETA_VENCIDA",

        "CVV_INVALIDO",

        "TOKEN_EXPIRADO",

        "TOKEN_INVALIDO",

        "AUTENTICACION_FALLIDA",

        "CONEXION_FALLIDA",

        "ERROR_BANCO",

        "ERROR_PROVEEDOR",

        "ERROR_BASE_DATOS",

        "ERROR_CONFIGURACION"
    ];


    for (
        const codigo of codigosConocidos
    ) {

        if (
            texto.includes(codigo)
        ) {

            return codigo;
        }
    }


    return "";
}


// =========================
// DETECTAR SERVICIO
// =========================

function detectarServicio(
    mensaje
) {

    const texto =
        mensaje.toLowerCase();


    if (
        texto.includes("banco") ||
        texto.includes("bancaria") ||
        texto.includes("bancario")
    ) {

        return "Banco";
    }


    if (
        texto.includes("pago") ||
        texto.includes("pagar") ||
        texto.includes("pagando")
    ) {

        return "Pago";
    }


    if (
        texto.includes("tarjeta") ||
        texto.includes("cvv")
    ) {

        return "Tarjeta";
    }


    if (
        texto.includes("token") ||
        texto.includes("autenticación") ||
        texto.includes("autenticacion")
    ) {

        return "Autenticacion";
    }


    return "General";
}


// =========================
// VISOR DE IMAGEN
// =========================

if (
    imageModal &&
    modalImage &&
    closeImageModalButton
) {

    // Cerrar mediante el botón X
    closeImageModalButton.addEventListener(
        "click",
        cerrarVisorImagen
    );


    // Cerrar haciendo clic en el fondo
    imageModal.addEventListener(
        "click",
        (event) => {

            if (
                event.target === imageModal
            ) {

                cerrarVisorImagen();
            }
        }
    );
}


// =========================
// CERRAR VISOR DE IMAGEN
// =========================

function cerrarVisorImagen() {

    if (
        !imageModal ||
        !modalImage
    ) {
        return;
    }

    imageModal.classList.remove(
        "active"
    );

    modalImage.src = "";
}