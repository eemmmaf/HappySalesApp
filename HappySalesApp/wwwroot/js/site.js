// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
"use strict";


window.onload = init;

//Anropar metoden hamburgerMenu när sidan initieras
function init() {
    hamburgerMenu();
}


//Funktion för att visa och dölja menyn 
const menuToggle = document.getElementById('menu-toggle');
const menu = document.getElementById('menu');
//Variabler för hamburger-menyn
let hamburger = document.getElementById("hamburger-icon");
let navUl = document.getElementById("nav-ul");

if (menuToggle) { 
    menuToggle.addEventListener('click', function () {
        if (menu.style.display === 'none') {
            menu.style.display = 'block';
        } else {
            menu.style.display = 'none';
        }
    })
};

//Funktion för hamburgermeny i huvudmenyn
function hamburgerMenu() {
    //Funktion för hamburger-menyn
    hamburger.addEventListener("click", () => {
        navUl.classList.toggle("show");
    })
}