(function (namespace) {
    //Constructor    
    function App() {
        this.Constructor();
    }
    App.STARTTIME = new Date();
    // Variables
    var _Tracert = true;


    // Métodos
    App.prototype.Constructor = function (turnOnTracert) {
        if (typeof turnOnTracert !== "undefined") { _Tracert = tracerOn; }

        var self = this;


        if (this.VersionIE && this.VersionIE < 8) {
            document.location.href = "/Support/noie6.htm";
        }

        this.Utils.KeyBoard();
        this.Utils.DisplayWhenEditing();
        if (_Tracert) { console.log("App inicializado correctamente..." + this.Runtime(App.STARTTIME)); }
    };

    App.prototype.Utils = {
        NoEnter: function () {
            if (_Tracert) { console.log('metodo: "App.Utils.NoEnter()" ha cargado exitosamente'); }
            return !(window.event && window.event.keyCode == 13)
        },
        ValidarCampos: function (idContentPlaceHolder, applyClass) {
            if (_Tracert) { console.log('metodo: "App.Utils.ValidarCampos()" ha cargado exitosamente'); }
            /// <summary>Permite validar todos los elemento de tipo TEXT, FILE, TEXTAREA y SELECT</summary>  
            /// <param name="idContentPlaceHolder" type="string">Id del contenedor de los elementos a evaluar, sino se especifica tomará por defecto el "document"</param>            
            var contenedor;
            if (idContentPlaceHolder != null && idContentPlaceHolder.length > 0)
                contenedor = document.getElementById(idContentPlaceHolder);
            else
                contenedor = document;
            var vacios = [];
            var obj = null;
            var inputs = contenedor.querySelectorAll("input[type=text]");
            var files = contenedor.querySelectorAll("input[type=file]");
            var textAreas = contenedor.getElementsByTagName("textarea");
            var selects = contenedor.getElementsByTagName("select");
            var objects = [];
            objects.push.apply(objects, inputs);
            objects.push.apply(objects, files);
            objects.push.apply(objects, textAreas);
            objects.push.apply(objects, selects);
            for (i = 0; i < objects.length; i++) {
                obj = objects[i];
                if (!obj.disabled)
                    if (obj.getAttribute("optional") == null) //Si tiene atributo opcional no validará
                        if (obj.value.length == 0 ) // Valida si es TEXTO que no este vacio y si es numero que sea mayor a 0
                        {
                            if (applyClass) {                                
                                this.ClassCss.Add(obj,"requerido" );
                            }

                            if (obj.getAttribute("title") != null)
                                vacios.push(obj.getAttribute("title").toUpperCase());
                            else
                                vacios.push("ID: " + obj.id.toUpperCase());
                        } else
                            this.ClassCss.Remove(obj,"requerido");

            }
            if (vacios.length > 0) {
                if (!applyClass) alert("ATENCIÓN: Hay un(os) campo(s) vacio(s):\r\r" + vacios.toString().replace(/,/g, '\r') + "\r\rPor favor ingrese la información y vuelva a intentarlo.");
                if (_Tracert) console.log("App.Utils.ValidarCampos(): Elementos vacios " + vacios.toString());
                /* Chequea si tiene un contendor como un DIV*/
                for (i = 0; i < objects.length; i++) {
                    obj = objects[i];
                    if (!obj.disabled)
                        if (obj.getAttribute("optional") == null) //Si tiene atributo opcional no validará
                            if (obj.value.length == 0)
                                //if (isNaN(obj.value) ? obj.value.length == 0 : parseInt(obj.value) < 0) // Valida si es TEXTO que no este vacio y si es numero que sea mayor a 0
                            {
                                var objContent = obj.parentElement;
                                if (objContent != null)
                                    if (objContent.style.display == 'none')
                                        objContent.style.display = 'block';
                                break;
                            }
                }
                return false;
            }
        },
        NoRefresh: function () {
            if (_Tracert) { console.log('metodo: "App.Utils.NoRefresh()" ha cargado exitosamente'); }
            document.onkeydown = function (e) {
                var key;
                if (window.event) {
                    key = event.keyCode;
                }
                else {
                    var unicode = e.keyCode ? e.keyCode : e.charCode;
                    key = unicode;
                }
                switch (key) {
                    case 116:
                        event.returnValue = false;
                        key = 0;
                        return false;
                    case 82:
                        if (event.ctrlKey) {
                            event.returnValue = false;
                            key = 0;
                            return false;
                        }
                        return false;
                    default:
                        return true;
                }
            };
        },
        ClassCss: {
            HasClass: function (elemento, clase) {
                if (_Tracert) { console.log('metodo: "App.Utils.ClassCss.HasClass()" ha cargado exitosamente'); }
                return new RegExp('(\\s|^)' + clase + '(\\s|$)').test(elemento.className);
            },
            Add: function (elemento, clase) {
                if (_Tracert) { console.log('metodo: "App.Utils.ClassCss.Add()" ha cargado exitosamente'); }
                if (!this.HasClass(elemento, clase)) { elemento.className += (elemento.className ? ' ' : '') + clase; }
            },
            Remove: function (elemento, clase) {
                if (_Tracert) { console.log('metodo: "App.Utils.ClassCss.Remove()" ha cargado exitosamente'); }
                if (this.HasClass(elemento, clase)) {
                    elemento.className = elemento.className.replace(new RegExp('(\\s|^)' + clase + '(\\s|$)'), ' ').replace(/^\s+|\s+$/g, '');
                }
            }
        },
        Toogle: function (elemento) {
            if (_Tracert) { console.log('metodo: "App.Utils.Toogle()" ha cargado exitosamente'); }
            var el = document.getElementById(elemento);
            if (el.style.display == "block") {
                el.style.display = "none";
            } else {
                el.style.display = "block";
            }
        },
        DisplayWhenEditing: function () {
            if (_Tracert) { console.log('metodo: "App.Utils.DisplayWhenEditing()" ha cargado exitosamente'); }
            var id = document.getElementById("ctl00_CPH_BODY_txtId");
            if (id != null && id.value > 0) {
                this.Toogle('editPanel');
            }
        },
        GetFecha: function (elemento) {            
                var obj = document.getElementById(elemento);
                if (obj != null) {
                    var date = new Date();
                    var str = this.LPad(date.getDate(), 2) + "-" + this.LPad((date.getMonth() + 1), 2) + "-" + date.getFullYear() + " " + this.LPad(date.getHours(), 2) + ":" + this.LPad(date.getMinutes(), 2) + ":" + this.LPad(date.getSeconds(), 2);
                    obj.value = str;
                }            
        },
        LPad:function (value, padding) {
            var zeroes = "0";
            for (var i = 0; i < padding; i++) { zeroes += "0"; }
            return (zeroes + value).slice(padding * -1);
        },
        KeyBoard: function () {
            if (_Tracert) { console.log('metodo: "App.Utils.KeyBoard()" ha cargado exitosamente'); }
            var self = this;
            document.onkeydown = function (e) {
                var key;
                if (window.event) {
                    key = event.keyCode
                }
                else {
                    var unicode = e.keyCode ? e.keyCode : e.charCode
                    key = unicode
                }
                switch (key.toString()) {
                    case "116": //F5
                        event.returnValue = false;
                        key = 0;
                        return false;
                    case "82": //R button
                        if (event.ctrlKey) {
                            event.returnValue = false;
                            key = 0;
                            return false;
                        }
                        break;
                    case "120": //F9
                        event.returnValue = false;
                        key = 0;
                        self.Toogle('editPanel');
                        var txts = document.getElementsByClassName("form-control");
                        txts[1].focus();
                        return false;
                }
            }
        }
    };

    App.prototype.UI = {
       
    };

    App.prototype.VersionIE = function () {
        var myNav = navigator.userAgent.toLowerCase();
        return (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1], 0) : false;
    };

    App.prototype.Runtime = function (starTime) {
        return (((new Date() - starTime) / 1000).toFixed(2) + " segundos...");
    };

    Object.defineProperty(App.prototype, "Tracert", {
        get: function Tracert() {
            return _Tracert;
        },
        set: function Tracert(value) {
            _Tracert = value;
        }
    });

    namespace.App = App;
}(window = window || {}));

window.onload = function () {
    this.app = new App();
};
