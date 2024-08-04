// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let btnLike = document.querySelectorAll(".likeIcon");

btnLike.forEach((btn) => {
    console.log(btn.classList.contains("bi-heart"))
    btn.addEventListener("click", (el) => {
        

        if (el.classList.contains("bi-heart")) {
            alert("Yes");
            icon.classList.replace("bi-heart", "bi-heart-fill");
        }else {
            icon.classList.replace("bi-heart-fill", "bi-heart")
        }
    })
    
})
