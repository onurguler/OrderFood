(function ($) {
  var OgPopup = function (options) {
    this.name = options.name ? options.name : "";
    this.title = options.title ? options.title : "";
    this.size = options.size ? options.size : "";
    this.autoShow = options.autoShow ? true : false;
    this.centered = options.centered ? true : false;
    this.body = options.body ? options.body : "";
    this.footer = options.footer ? options.footer : {};
    this.element = null;
    this.popupId = null;
    this.onCreated = options.onCreated;
    this.onConfirmed = options.onConfirmed;
    this.onAjaxContentLoaded = options.onAjaxContentLoaded;
    this.ajax = options.ajax;
  };

  $.fn.ogPopup = function (options) {
    var object = new OgPopup(options);
    var element = object.init();
    object.element = element;
    element.data("ogPopup", object);
    if (object.autoShow) {
      object.show();
    }
    if (object.onCreated) {
      object.onCreated(object);
    }
    if (object.ajax) {
      object.loadAjaxContent();
    }
  };

  OgPopup.prototype.init = function () {
    var self = this;

    var popupId = this.name + "_ogPopup";
    this.popupId = popupId;

    var popupsDiv = this.getOgPopupsDiv();

    var modal = document.createElement("div");
    modal.setAttribute("id", popupId);
    modal.setAttribute("class", "modal fade");
    modal.setAttribute("tabindex", "-1");

    var modalDialog = document.createElement("div");
    modalDialog.classList.add("modal-dialog");
    if (this.centered) {
      modalDialog.classList.add("modal-dialog-centered");
    }
    if (this.size) {
      modalDialog.classList.add("modal-" + this.size);
    }

    modal.appendChild(modalDialog);

    var modalContent = document.createElement("div");
    modalContent.setAttribute("class", "modal-content");

    modalDialog.appendChild(modalContent);

    var modalHeader = document.createElement("modal-header");
    modalHeader.setAttribute("class", "modal-header");

    modalContent.appendChild(modalHeader);

    var modalTitle = document.createElement("h5");
    modalTitle.setAttribute("class", "modal-title");
    modalTitle.innerText = this.title;

    modalHeader.appendChild(modalTitle);

    var modalClose = document.createElement("button");
    modalClose.setAttribute("type", "button");
    modalClose.setAttribute("class", "close");
    modalClose.setAttribute("data-dismiss", "modal");
    modalClose.setAttribute("aria-label", "Close");
    modalClose.innerHTML = '<span aria-hidden="true">&times;</span>';

    modalHeader.appendChild(modalClose);

    var modalBody = document.createElement("div");
    modalBody.setAttribute("class", "modal-body");
    modalBody.innerHTML = this.body;

    modalContent.appendChild(modalBody);

    if ($.isPlainObject(this.footer)) {
      var commands = this.footer.commands;

      if (
        commands &&
        $.isArray(commands) &&
        commands.length &&
        commands.length > 0
      ) {
        var modalFooter = document.createElement("div");
        modalFooter.setAttribute("class", "modal-footer");

        for (var i = 0; i < commands.length; i++) {
          var commandButton = document.createElement("button");
          commandButton.setAttribute("type", "button");
          commandButton.classList.add("btn");

          var command = commands[i];
          var name = command;

          if ($.isPlainObject(command)) {
            name = command.name;
          }

          switch (name) {
            case "close":
              commandButton.classList.add("btn-secondary");
              commandButton.setAttribute("data-dismiss", "modal");
              var title = "Close";
              if ($.isPlainObject(command)) {
                title = command.title ? command.title : title;
              }
              commandButton.innerText = title;
              commandButton.onclick = function (e) {
                if (self.onConfirmed && $.isFunction(self.onConfirmed)) {
                  self.onConfirmed(e, "canceled", self);
                }
              };
              break;
            case "save":
              commandButton.classList.add("btn-primary");
              var title = "Save Changes";
              if ($.isPlainObject(command)) {
                title = command.title ? command.title : title;
              }
              commandButton.innerText = title;
              commandButton.onclick = function (e) {
                if (self.onConfirmed && $.isFunction(self.onConfirmed)) {
                  self.onConfirmed(e, "confirmed", self);
                }
              };
              break;
            default:
              commandButton.setAttribute("id", popupId + "_footer_" + name);
              commandButton.innerText = command.title;
              break;
          }

          modalFooter.appendChild(commandButton);
        }

        modalContent.appendChild(modalFooter);
      }
    }

    popupsDiv.appendChild(modal);

    $("#" + popupId).on("hidden.bs.modal", function () {
      $(this).remove();
    });

    return $("#" + popupId);
  };

  OgPopup.prototype.getOgPopupsDiv = function () {
    var popupsDiv = document.getElementById("ogPopups");

    if (!popupsDiv) {
      popupsDiv = document.createElement("div");
      popupsDiv.setAttribute("id", "ogPopups");
      document.body.appendChild(popupsDiv);
    }

    return popupsDiv;
  };

  OgPopup.prototype.show = function () {
    this.element.modal({ show: true });
  };

  OgPopup.prototype.hide = function () {
    this.element.modal("hide");
  };

  OgPopup.prototype.destroy = function () {
    var modal = document.getElementById("#" + this.popupId);
    modal.parentNode.removeChild(modal);
  };

  OgPopup.prototype.loadAjaxContent = function () {
    var self = this;
    var modalBody = self.element.find(".modal-body");
    modalBody.html("Loading...");
    $.get(this.ajax).then(function (response) {
      modalBody.html(response);
      if (self.onAjaxContentLoaded) {
        self.onAjaxContentLoaded(self);
      }
    });
  };
})(jQuery);
