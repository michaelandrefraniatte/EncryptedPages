
var firstcheck = false;

const txt = `
<head>

    <meta charset="UTF-8">
    <meta name="description" content="A gallery of contents for favours to edit, achieved with JavaScript.">
    <meta name="keywords" content="my-favours, player, photos, gallery, videos-games, films, zapping">
    <meta name="author" content="MICHAEL ANDRE FRANIATTE">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <link rel="shortcut icon" type="image/png" href="img/favicon.png" />
    <meta name="google-site-verification" content="7cncMfoTo9D6fhKexNpS1eSgDPB9dqBjn4KojNDuHR0" />

    <title>Michael Franiatte PP</title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.css">
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <style>
                body {
                    font-family: sans-serif;
                    height: 100%;
                    background-image: url("img/background.jpg");
                    background-repeat: no-repeat;
                    background-attachment: fixed;
                    background-position: center;
                    overflow-x: hidden;
                }
    </style>
</head>
<body>

    <img src="logo.jpg"></img>

    <script>
	alert("ok");
    </script>

</body>
`;

function onSubmit(form) {
    if (!firstcheck) {
        firstcheck = true;
        const password = $('#password').val() || form.token;
	alert(password);
        var content = txt;
	$("myfile").empty().append(content);
    }
}