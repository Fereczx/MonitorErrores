
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

const errorReview = document.getElementById("error-review");
const reviewCode = document.getElementById("review-code");
const reviewService = document.getElementById("review-service");
const reviewMessage = document.getElementById("review-message");
const reviewSituation = document.getElementById("review-situation");
const cancelReviewButton = document.getElementById("cancel-review");
const confirmReviewButton = document.getElementById("confirm-review");

const removeImageButton = document.getElementById("remove-image");
const analyzeImageButton = document.getElementById("analyze-image");

const imageModal = document.getElementById("image-modal");
const modalImage = document.getElementById("modal-image");
const closeImageModalButton = document.getElementById("close-image-modal");

errorReview.style.display = "none";

const API_URL = "http://localhost:5163/api/Errores";
const API_IMAGE_URL = "http://localhost:5163/api/Errores/imagen";

let imagenSeleccionada = null;
let imagenPreviewUrl = null;

// =========================
// ABRIR, CERRAR Y MINIMIZAR
// =========================

chatButton.addEventListener("click", () => {
    chatWindow.style.display = "flex";
    chatButton.style.display = "none";
    messageInput.focus();
});

closeButton.addEventListener("click", () => {
    chatWindow.style.display = "none";
    chatButton.style.display = "flex";
});

minimizeButton.addEventListener("click", () => {
    chatWindow.style.display = "none";
    chatButton.style.display = "flex";
});

// =========================
// MANEJO DE IMÁGENES
// =========================

attachButton.addEventListener("click", () => {
    imageInput.click();
});

imageInput.addEventListener("change", () => {
    if (imageInput.files.length === 0) {
        return;
    }

    mostrarVistaPrevia(imageInput.files[0]);
});

function mostrarVistaPrevia(archivo) {
    imagenSeleccionada = archivo;

    if (imagenPreviewUrl) {
        URL.revokeObjectURL(imagenPreviewUrl);
    }

    imagenPreviewUrl = URL.createObjectURL(archivo);
    previewImage.src = imagenPreviewUrl;
    imageName.textContent = archivo.name;
    imagePreview.classList.add("active");
}

removeImageButton.addEventListener("click", () => {
    limpiarVistaPrevia();
});

function limpiarVistaPrevia() {
    imagenSeleccionada = null;

    if (imagenPreviewUrl) {
        URL.revokeObjectURL(imagenPreviewUrl);
        imagenPreviewUrl = null;
    }

    previewImage.src = "";
    imageName.textContent = "";
    imagePreview.classList.remove("active");
    imageInput.value = "";
}

analyzeImageButton.addEventListener("click", analizarImagen);

async function analizarImagen() {
    if (!imagenSeleccionada) {
        return;
    }

    const archivo = imagenSeleccionada;

    limpiarVistaPrevia();
    agregarImagenUsuario(archivo);
    setEstadoEnvio(false);

    const mensajeCarga = agregarMensajeAnalizando();

    try {
        const formData = new FormData();
        formData.append("Imagen", archivo);

        console.log("Enviando imagen al backend:", archivo.name);

        const respuesta = await fetch(API_IMAGE_URL, {
            method: "POST",
            body: formData
        });

        if (!respuesta.ok) {
            throw new Error(`Error HTTP ${respuesta.status}`);
        }

        const resultado = await respuesta.json();

        console.log("Respuesta del análisis de imagen:", resultado);

        mensajeCarga.remove();
        mostrarResultado(resultado);
    } catch (error) {
        console.error("Error analizando la imagen:", error);

        mensajeCarga.remove();

        agregarMensajeBot(
            "No pudimos analizar la imagen. Por favor, intentá nuevamente."
        );
    } finally {
        setEstadoEnvio(true);
        messageInput.focus();
    }
}

// =========================
// ENTRADA DE MENSAJES
// =========================

sendButton.addEventListener("click", enviarMensaje);

messageInput.addEventListener("keydown", (event) => {
    if (event.key === "Enter" && !event.shiftKey) {
        event.preventDefault();
        enviarMensaje();
    }
});

// =========================
// PREGUNTAS ADAPTATIVAS
// =========================

function configurarPregunta(mensaje) {
    const texto = mensaje.toLowerCase();

    const etiquetaSituacion = document.querySelector(
        'label[for="review-situation"]'
    );

    reviewSituation.innerHTML = "";

    let pregunta;
    let opciones;

    if (
        texto.includes("pendiente") ||
        texto.includes("demora") ||
        texto.includes("demorado")
    ) {
        pregunta = "¿Desde cuándo está pendiente o demorada la operación?";

        opciones = [
            ["minutos", "Hace unos minutos"],
            ["horas", "Hace varias horas"],
            ["dias", "Desde ayer o antes"],
            ["desconocido", "No estoy seguro"]
        ];
    } else if (
        texto.includes("404") ||
        texto.includes("500") ||
        texto.includes("503") ||
        detectarCodigo(mensaje) !== ""
    ) {
        pregunta = "¿En qué momento aparece el error?";

        opciones = [
            ["inicio", "Al ingresar al sistema"],
            ["operacion", "Al realizar una operación"],
            ["final", "Al finalizar la operación"],
            ["desconocido", "No estoy seguro"]
        ];
    } else if (
        texto.includes("pago") ||
        texto.includes("pagar") ||
        texto.includes("tarjeta")
    ) {
        pregunta = "¿Qué sucede cuando intentás realizar el pago?";

        opciones = [
            ["rechazado", "La operación es rechazada"],
            ["pendiente", "La operación queda pendiente"],
            ["error", "Aparece un mensaje de error"],
            ["sin_respuesta", "La pantalla no responde"],
            ["otro", "Sucede otra cosa"],
            ["desconocido", "No estoy seguro"]
        ];
    } else {
        pregunta = "¿Podés contarme cuándo ocurre el problema?";

        opciones = [
            ["inicio", "Al ingresar al sistema"],
            ["operacion", "Al realizar una operación"],
            ["final", "Al finalizar la operación"],
            ["desconocido", "No estoy seguro"]
        ];
    }

    etiquetaSituacion.textContent = pregunta;

    opciones.forEach(([valor, textoOpcion]) => {
        const opcion = document.createElement("option");
        opcion.value = valor;
        opcion.textContent = textoOpcion;
        reviewSituation.appendChild(opcion);
    });

    reviewSituation.value = "desconocido";
}

// =========================
// PREPARAR MENSAJE PARA REVISIÓN
// =========================

function enviarMensaje() {
    const mensaje = messageInput.value.trim();

    if (mensaje === "") {
        return;
    }

    reviewCode.value = detectarCodigo(mensaje);
    reviewService.value = detectarServicio(mensaje);
    reviewMessage.value = mensaje;

    configurarPregunta(mensaje);

    errorReview.style.display = "block";
    messageInput.value = "";

    errorReview.scrollIntoView({
        behavior: "smooth",
        block: "nearest"
    });
}

// =========================
// CANCELAR REVISIÓN
// =========================

cancelReviewButton.addEventListener("click", () => {
    messageInput.value = reviewMessage.value;

    errorReview.style.display = "none";
    messageInput.focus();
});

// =========================
// CONFIRMAR Y ENVIAR DIAGNÓSTICO
// =========================

confirmReviewButton.addEventListener("click", async () => {
    const situacion = reviewSituation.value;

    const textoSituacion =
        reviewSituation.options[reviewSituation.selectedIndex]?.text || "";

    const etiquetaSituacion = document.querySelector(
        'label[for="review-situation"]'
    );

    const preguntaSituacion = etiquetaSituacion
        ? etiquetaSituacion.textContent.trim()
        : "";

    const mensajeOriginal = reviewMessage.value.trim();

    if (mensajeOriginal === "") {
        agregarMensajeBot(
            "Ingresá un mensaje de error antes de continuar."
        );
        return;
    }

    const respuestaValida =
        situacion !== "" && situacion !== "desconocido";

    const error = {
        codigo: reviewCode.value.trim(),
        servicio: reviewService.value,
        mensaje: mensajeOriginal
    };

    // El mensaje original se conserva.
    // La pregunta y la respuesta se agregan al contexto enviado a la API.
    if (respuestaValida) {
        error.mensaje +=
            "\n" + preguntaSituacion +
            "\nRespuesta del usuario: " + textoSituacion;
    }

    errorReview.style.display = "none";

    // Mostrar el mensaje original y la información adicional por separado.
    agregarMensajeUsuarioConContexto(
        mensajeOriginal,
        respuestaValida ? preguntaSituacion : "",
        respuestaValida ? textoSituacion : ""
    );

    setEstadoEnvio(false);

    const mensajeCarga = agregarMensajeAnalizando();

    try {
        const respuesta = await fetch(API_URL, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(error)
        });

        if (!respuesta.ok) {
            throw new Error(`Error HTTP ${respuesta.status}`);
        }

        const resultado = await respuesta.json();

        mensajeCarga.remove();
        mostrarResultado(resultado);
    } catch (error) {
        console.error(
            "Error comunicándose con el backend:",
            error
        );

        mensajeCarga.remove();

        agregarMensajeBot(
            "No pudimos comunicarnos con el sistema. Por favor, intentá nuevamente."
        );
    } finally {
        setEstadoEnvio(true);
        messageInput.focus();
    }
});

// =========================
// MOSTRAR MENSAJE DEL USUARIO
// CON INFORMACIÓN ADICIONAL
// =========================

function agregarMensajeUsuarioConContexto(
    mensajeOriginal,
    pregunta,
    respuesta
) {
    const elemento = document.createElement("div");
    elemento.classList.add("user-message-container");

    const mensaje = document.createElement("div");
    mensaje.classList.add("message", "user-message");
    mensaje.textContent = mensajeOriginal;

    elemento.appendChild(mensaje);

    if (pregunta && respuesta) {
        const contexto = document.createElement("div");
        contexto.classList.add("user-context");

        const titulo = document.createElement("strong");
        titulo.textContent = "Información adicional";

        const preguntaTexto = document.createElement("p");
        preguntaTexto.textContent = pregunta;

        const respuestaTexto = document.createElement("div");
        respuestaTexto.classList.add("user-context-answer");
        respuestaTexto.textContent = respuesta;

        contexto.appendChild(titulo);
        contexto.appendChild(preguntaTexto);
        contexto.appendChild(respuestaTexto);

        elemento.appendChild(contexto);
    }

    chatMessages.appendChild(elemento);
    desplazarChatAlFinal();
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

    const solucionCompleta =
        resultado.solucionCompleta === true ||
        (
            resultado.origen === "conocimiento" &&
            resultado.confirmada === true
        );

    if (solucionCompleta) {
        const contenedor = crearContenedorMensajeBot();

        contenedor.mensaje.classList.add("solution-message");

        // Problema detectado
        if (resultado.problema) {
            const seccionProblema = document.createElement("div");
            seccionProblema.classList.add("result-section");

            const titulo = document.createElement("span");
            titulo.classList.add("result-title");
            titulo.textContent = "Problema detectado";

            const texto = document.createElement("div");
            texto.textContent = resultado.problema;

            seccionProblema.appendChild(titulo);
            seccionProblema.appendChild(texto);

            contenedor.mensaje.appendChild(seccionProblema);
        }

        // Solución o mensaje para el usuario
        const mensajeSolucion =
            resultado.mensajeUsuario || resultado.solucion;

        if (mensajeSolucion) {
            const seccionSolucion = document.createElement("div");
            seccionSolucion.classList.add("result-section");

            const titulo = document.createElement("span");
            titulo.classList.add("result-title");
            titulo.textContent = "Solución";

            const texto = document.createElement("div");
            texto.textContent = mensajeSolucion;

            seccionSolucion.appendChild(titulo);
            seccionSolucion.appendChild(texto);

            contenedor.mensaje.appendChild(seccionSolucion);
        }

        chatMessages.appendChild(contenedor.elemento);
        desplazarChatAlFinal();

        return;
    }


    // Orientación preliminar generada por la IA
    if (
        resultado.origen === "ia" &&
        resultado.solucionCompleta === false &&
        resultado.solucion &&
        resultado.mensajeUsuario
    ) {
        const contenedor = crearContenedorMensajeBot();

        contenedor.mensaje.classList.add("solution-message");

        const tituloOrientacion = document.createElement("div");
        tituloOrientacion.classList.add("result-title");
        tituloOrientacion.textContent = "Orientación preliminar";

        contenedor.mensaje.appendChild(tituloOrientacion);

        if (resultado.problema) {
            const seccionProblema = document.createElement("div");
            seccionProblema.classList.add("result-section");

            const titulo = document.createElement("span");
            titulo.classList.add("result-title");
            titulo.textContent = "Problema detectado";

            const texto = document.createElement("div");
            texto.textContent = resultado.problema;

            seccionProblema.appendChild(titulo);
            seccionProblema.appendChild(texto);

            contenedor.mensaje.appendChild(seccionProblema);
        }

        const seccionSolucion = document.createElement("div");
        seccionSolucion.classList.add("result-section");

        const tituloSolucion = document.createElement("span");
        tituloSolucion.classList.add("result-title");
        tituloSolucion.textContent = "Pasos recomendados";

        const textoSolucion = document.createElement("div");
        textoSolucion.textContent = resultado.solucion;

        seccionSolucion.appendChild(tituloSolucion);
        seccionSolucion.appendChild(textoSolucion);

        contenedor.mensaje.appendChild(seccionSolucion);

        const aviso = document.createElement("div");
        aviso.classList.add("result-section");
        aviso.textContent =
            "Esta orientación no confirma la causa exacta del error. " +
            "Si el problema continúa, contactá a Atención al Cliente.";

        contenedor.mensaje.appendChild(aviso);


        chatMessages.appendChild(contenedor.elemento);
        desplazarChatAlFinal();

        return;
    }


    // Atención al Cliente
    if (resultado.mensajeUsuario) {
        const contenedor = crearContenedorMensajeBot();

        contenedor.mensaje.classList.add("customer-service-message");

        const titulo = document.createElement("span");
        titulo.classList.add("result-title");
        titulo.textContent = "Atención al Cliente";

        const texto = document.createElement("div");
        texto.textContent = resultado.mensajeUsuario;

        contenedor.mensaje.appendChild(titulo);
        contenedor.mensaje.appendChild(texto);

        chatMessages.appendChild(contenedor.elemento);
        desplazarChatAlFinal();

        return;
    }

    agregarMensajeBot(
        "No pudimos determinar una solución para este problema. Por favor, comunicate con Atención al Cliente para recibir asistencia."
    );
}

// =========================
// CONTENEDOR DE MENSAJE DEL BOT
// =========================

function crearContenedorMensajeBot() {
    const elemento = document.createElement("div");
    elemento.classList.add("bot-message-container");

    const avatar = document.createElement("div");
    avatar.classList.add("bot-avatar");
    avatar.textContent = "M";

    const mensaje = document.createElement("div");
    mensaje.classList.add("message", "bot-message");

    elemento.appendChild(avatar);
    elemento.appendChild(mensaje);

    return {
        elemento,
        mensaje
    };
}

// =========================
// MENSAJE SIMPLE DEL USUARIO
// =========================

function agregarMensajeUsuario(mensaje) {
    const elemento = document.createElement("div");

    elemento.classList.add("message", "user-message");
    elemento.textContent = mensaje;

    chatMessages.appendChild(elemento);
    desplazarChatAlFinal();
}

// =========================
// IMAGEN DEL USUARIO EN EL CHAT
// =========================

function agregarImagenUsuario(archivo) {
    const elemento = document.createElement("div");
    elemento.classList.add("message", "user-message");

    const imagen = document.createElement("img");
    imagen.classList.add("chat-image");

    const url = URL.createObjectURL(archivo);

    imagen.src = url;
    imagen.alt = "Captura enviada";

    imagen.addEventListener("click", () => {
        if (!imageModal || !modalImage) {
            return;
        }

        modalImage.src = url;
        imageModal.classList.add("active");
    });

    elemento.appendChild(imagen);
    chatMessages.appendChild(elemento);

    desplazarChatAlFinal();
}

// =========================
// MENSAJE DEL BOT
// =========================

function agregarMensajeBot(mensaje) {
    const contenedor = document.createElement("div");
    contenedor.classList.add("bot-message-container");

    const avatar = document.createElement("div");
    avatar.classList.add("bot-avatar");
    avatar.textContent = "M";

    const elemento = document.createElement("div");
    elemento.classList.add("message", "bot-message");
    elemento.textContent = mensaje;

    contenedor.appendChild(avatar);
    contenedor.appendChild(elemento);

    chatMessages.appendChild(contenedor);
    desplazarChatAlFinal();

    return contenedor;
}

// =========================
// INDICADOR DE ANÁLISIS
// =========================

function agregarMensajeAnalizando() {
    const contenedor = document.createElement("div");
    contenedor.classList.add("bot-message-container");

    const avatar = document.createElement("div");
    avatar.classList.add("bot-avatar");
    avatar.textContent = "M";

    const mensaje = document.createElement("div");
    mensaje.classList.add(
        "message",
        "bot-message",
        "typing-message"
    );

    for (let i = 0; i < 3; i++) {
        const punto = document.createElement("span");
        punto.classList.add("typing-dot");
        mensaje.appendChild(punto);
    }

    contenedor.appendChild(avatar);
    contenedor.appendChild(mensaje);

    chatMessages.appendChild(contenedor);
    desplazarChatAlFinal();

    return contenedor;
}

// =========================
// DESPLAZAR CHAT AL FINAL
// =========================

function desplazarChatAlFinal() {
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

// =========================
// ACTIVAR / DESACTIVAR ENVÍO
// =========================

function setEstadoEnvio(activado) {
    sendButton.disabled = !activado;
    messageInput.disabled = !activado;
    attachButton.disabled = !activado;
    analyzeImageButton.disabled = !activado;
}

// =========================
// DETECTAR CÓDIGO DE ERROR
// =========================

function detectarCodigo(mensaje) {
    const texto = mensaje.toUpperCase();

    const coincidenciaHttp = texto.match(
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

    for (const codigo of codigosConocidos) {
        if (texto.includes(codigo)) {
            return codigo;
        }
    }

    return "";
}

// =========================
// DETECTAR SERVICIO
// =========================


function detectarServicio(mensaje) {
    const texto = mensaje
        .toLowerCase()
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "");

    // Errores de autenticación
    if (
        texto.includes("401") ||
        texto.includes("403") ||
        texto.includes("unauthorized") ||
        texto.includes("forbidden") ||
        texto.includes("autenticacion") ||
        texto.includes("token") ||
        texto.includes("inicio de sesion") ||
        texto.includes("iniciar sesion") ||
        texto.includes("credenciales")
    ) {
        return "Autenticacion";
    }

    // Errores específicos de tarjetas
    if (
        texto.includes("tarjeta_rechazada") ||
        texto.includes("tarjeta rechazada") ||
        texto.includes("tarjeta vencida") ||
        texto.includes("cvv") ||
        texto.includes("tarjeta") ||
        texto.includes("medio de pago")
    ) {
        return "Tarjeta";
    }

    // Errores relacionados con pagos
    if (
        texto.includes("pago") ||
        texto.includes("pagar") ||
        texto.includes("pagando") ||
        texto.includes("transaccion") ||
        texto.includes("operacion pendiente") ||
        texto.includes("cobro")
    ) {
        return "Pagos";
    }

    // Errores bancarios
    if (
        texto.includes("banco") ||
        texto.includes("bancaria") ||
        texto.includes("bancario")
    ) {
        return "Banco";
    }

    // Errores de conexión
    if (
        texto.includes("timeout") ||
        texto.includes("conexion") ||
        texto.includes("sin respuesta")
    ) {
        return "Red";
    }

    return "General";
}


// =========================
// VISOR DE IMAGEN AMPLIADA
// =========================

if (imageModal && modalImage && closeImageModalButton) {
    closeImageModalButton.addEventListener(
        "click",
        cerrarVisorImagen
    );

    imageModal.addEventListener("click", (event) => {
        if (event.target === imageModal) {
            cerrarVisorImagen();
        }
    });
}

function cerrarVisorImagen() {
    if (!imageModal || !modalImage) {
        return;
    }

    imageModal.classList.remove("active");
    modalImage.src = "";
}
