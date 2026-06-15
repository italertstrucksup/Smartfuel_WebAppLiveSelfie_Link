let stream = null;
let capturedBlob = null;

$(document).ready(async function () {
    $("#capturedImage").hide();

    $("#video").show();

    $(".face-overlay").show();

    $(".instruction-box").show();

    $("#capsus").hide();

    $("#btnCapture").show();
    $("#capturetxt").show();

    $("#btnRetake").hide();

    $("#btnSubmit").hide();
    try {

        stream = await navigator.mediaDevices.getUserMedia({
            video: {
                facingMode: "user",
                width: { ideal: 1280 },
                height: { ideal: 720 }
            },
            audio: false
        });

        $("#video")[0].srcObject = stream;

    }
    catch (e) {

        alert("Please allow camera access.");

    }

});

$("#btnCapture").click(function () {

    const video = $("#video")[0];
    const canvas = $("#canvas")[0];

    canvas.width = video.videoWidth;
    canvas.height = video.videoHeight;

    const ctx = canvas.getContext("2d");

    ctx.drawImage(
        video,
        0,
        0,
        canvas.width,
        canvas.height
    );

    canvas.toBlob(function (blob) {

        capturedBlob = blob;

        const imageUrl = URL.createObjectURL(blob);

        $("#capturedImage")
            .attr("src", imageUrl)
            .show();

        $("#video").hide();
        $(".face-overlay").hide();

        $(".instruction-box").hide();

        $("#capsus").show();

        $("#btnCapture").hide();
        $("#capturetxt").hide();

        $("#btnRetake").show();
        $("#btnSubmit").show();

    }, "image/jpeg", 0.9);

});

$("#btnRetake").click(function () {

    $("#capturedImage").hide();

    $("#video").show();

    $(".face-overlay").show();

    $(".instruction-box").show();

    $("#capsus").hide();

    $("#btnCapture").show();
    $("#capturetxt").show();

    $("#btnRetake").hide();

    $("#btnSubmit").hide();

});


$("#btnSubmit").click(function () {
    $("#loader").fadeIn();
    if (!capturedBlob) {
        alert("Please capture image first.");
        return;
    }

    const randomFileName =
        crypto.randomUUID() + ".jpg";

    let formData = new FormData();

    formData.append(
        "file",
        capturedBlob,
        randomFileName
    );

    $.ajax({
        url: '/LiveSelfie/UpdateImage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (result && result.statusCode == 200) {
                window.onbeforeunload = null;
                $("#btnRetake").hide();
                $("#capsus").hide();
                $("#btnSubmit").hide();
                $("#upsus").show();

            }
            else {
                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: result.message,
                    confirmButtonText: "OK"
                });
            }
        },
        error: function (error) {
            Swal.fire({
                icon: "error",
                title: error,
                timer: 1500
            });
        },
        complete: function () {
            $("#loader").fadeOut(); // ✅ always stop loader
        }
    })
});