// /*  ==========================================
//     SHOW UPLOADED IMAGE
// * ========================================== */
// function readURL(input) {
//   if (input.files && input.files[0]) {
//     var reader = new FileReader();

//     reader.onload = function (e) {
//       $("#imageResult").attr("src", e.target.result);
//     };
//     reader.readAsDataURL(input.files[0]);
//   }
// }

// $(function () {
//   $("#upload").on("change", function () {
//     readURL(input);
//   });
// });

// /*  ==========================================
//     SHOW UPLOADED IMAGE NAME
// * ========================================== */
// var input = document.getElementById("upload");
// var infoArea = document.getElementById("upload-label");

// input.addEventListener("change", showFileName);
// function showFileName(event) {
//   var input = event.srcElement;
//   var fileName = input.files[0].name;
//   infoArea.textContent = "File name: " + fileName;
// }

(function ($) {
  $.fn.initImageUpload = function () {
    console.log("calisti");
    var element = $(this);
    console.log(element);
    var input = element.find("input");
    console.log(input);
    var uploadLabel = element.find("label");
    console.log(uploadLabel);
    var img = element.find("img");
    console.log(img);

    input.on("change", function () {
      console.log("on changfe");
      if (input[0].files && input[0].files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
          img.attr("src", e.target.result);
        };
        reader.readAsDataURL(input[0].files[0]);
      }
      var fileName = input[0].files[0].name;
      uploadLabel.textContent = fileName;
    });
  };
})(jQuery);
