(function ($) {
  var OgBsTable = function (bsTableOptions, ogTableOptions) {
    this.bsTableOptions = bsTableOptions;
    this.create = ogTableOptions.create;
    this.read = ogTableOptions.read;
    this.update = ogTableOptions.update;
    this.delete = ogTableOptions.delete;
    this.searhableFields = ogTableOptions.searchableFields;
    this.sortableFields = ogTableOptions.sortableFields;
    this.idField = ogTableOptions.idField;
    this.element = null;
    this.onPopupContentLoaded = ogTableOptions.onPopupContentLoaded
      ? ogTableOptions.onPopupContentLoaded
      : [];
  };

  $.fn.ogBsTable = function (bsTableOptions, ogTableOptions) {
    var jqElement = $(this);

    var object = new OgBsTable(bsTableOptions, ogTableOptions);

    object.initBsTable(jqElement);
    object.element = jqElement;

    jqElement.data("ogBsTable", object);

    return jqElement;
  };

  OgBsTable.prototype.initBsTable = function (tableJqElement) {
    var self = this;

    this.initBsTableOptions();

    tableJqElement.bootstrapTable("destroy").bootstrapTable({
      pagination: true,
      sidePagination: "server",
      pageSize: 10,
      pageList: "[10, 25, 50, 100, all]",
      search: true,
      showRefresh: true,
      showToggle: false,
      showFullscreen: true,
      showColumns: true,
      showColumnsToggleAll: true,
      detailView: false,
      showExport: true,
      showPaginationSwitch: false,
      showFooter: false,
      ...self.bsTableOptions,
    });
  };

  OgBsTable.prototype.initBsTableOptions = function () {
    var self = this;

    if (this.read) {
      this.bsTableOptions.ajax = function (params) {
        var url = self.read;
        $.get(url + "?" + $.param(params.data)).then(function (res) {
          params.success(res);
        });
      };
    }

    if (this.update || this.delete) {
      this.bsTableOptions.columns.push({
        title: "",
        align: "center",
        clickToSelect: false,
        events: {
          "click .update": function (e, value, row, index) {
            operateUpdate(self, row[self.idField]);
          },
          "click .delete": function (e, value, row, index) {
            operateDelete(self, row[self.idField]);
          },
        },
        formatter: function (value, row, index) {
          return operationsFormatter(value, row, index, self);
        },
      });
    }

    if (this.create) {
      this.bsTableOptions.buttons = [
        {
          text: "Create",
          icon: "fas fa-plus",
          event: {
            click: function () {
              operateCreate(self);
            },
          },
        },
      ];
    }

    if (this.sortableFields) {
      var fields = this.sortableFields.split(",");
      for (var i = 0; i < fields.length; i++) {
        var field = fields[i].trim();
        for (var j = 0; j < this.bsTableOptions.columns.length; j++) {
          if (this.bsTableOptions.columns[j].field === field) {
            this.bsTableOptions.columns[j].sortable = true;
          }
        }
      }
    }

    this.bsTableOptions.idField = this.idField;
    this.bsTableOptions.uniqueId = this.idField;
  };

  function operationsFormatter(value, row, index, ogBsTable) {
    var opButtonsHtml = [];

    if (ogBsTable.update) {
      var updateButtonHtml = [
        '<a class="update btn btn-warning" href="javascript:void(0)" title="Update">',
        '<i class="fas fa-edit"></i> Update',
        "</a>",
      ];
      opButtonsHtml = [...opButtonsHtml, ...updateButtonHtml];
    }

    if (ogBsTable.delete) {
      var deleteButtonHtml = [
        '<a class="delete btn btn-danger" href="javascript:void(0)" title="Remove">',
        '<i class="fa fa-trash"></i> Delete',
        "</a>",
      ];
      opButtonsHtml = [...opButtonsHtml, ...deleteButtonHtml];
    }

    return opButtonsHtml.join("");
  }

  function operateCreate(ogBsTable) {
    $().ogPopup({
      ajax: ogBsTable.create,
      name: "create",
      title: "Create",
      size: "xl",
      centered: true,
      autoShow: true,
      footer: { commands: ["close", "save"] },
      onAjaxContentLoaded: function (popup) {
        var form = popup.element.find("form");
        form.submit(function (event) {
          $(this).removeData("validator").removeData("unobtrusiveValidation");
          $.validator.unobtrusive.parse($(this));
          event.preventDefault();

          if (form.valid()) {
            var action = $(this).attr("action");
            var requestMethod = $(this).attr("method");
            var formData = {};
            var addinitonalFormOptions = {};
            console.log(form.attr("enctype"));
            if (form.attr("enctype") === "multipart/form-data") {
              formData = new FormData(this);
              addinitonalFormOptions.enctype === form.attr("enctype");
              addinitonalFormOptions.processData = false;
              addinitonalFormOptions.contentType = false;
              addinitonalFormOptions.cache = false;
              addinitonalFormOptions.timeout = 600000;
            } else {
              var formDataRaw = $(this).serializeArray();
              for (var i = 0; i < formDataRaw.length; i++) {
                formData[formDataRaw[i].name] = formDataRaw[i].value;
              }
            }

            $.ajax({
              type: requestMethod,
              url: action,
              data: formData,
              ...addinitonalFormOptions,
              success: function (data, textStatus, jQxhr) {
                if ($.isPlainObject(data)) {
                  ogBsTable.element.bootstrapTable("insertRow", {
                    index: 0,
                    row: data.data,
                  });
                  ogBsTable.element.bootstrapTable(
                    "load",
                    ogBsTable.element.bootstrapTable("getData", {
                      useCurrentPage: true,
                      formatted: true,
                    })
                  );
                  popup.hide();
                  return;
                }
                $("#" + modalId)
                  .find(".modal-body")
                  .html(data);
              },
              error: function (jQxhr, textStatus, errorThrown) {
                console.log(errorThrown);
              },
            });
          }
        });

        if (ogBsTable.onPopupContentLoaded) {
          ogBsTable.onPopupContentLoaded(ogBsTable, popup, "create");
        }
      },
      onConfirmed: function (e, result, popup) {
        if (result === "confirmed") {
          var form = popup.element.find("form");
          form.submit();
        }
      },
    });
  }

  function operateUpdate(ogBsTable, id) {
    $().ogPopup({
      ajax: ogBsTable.update + "/" + id,
      name: "updaate",
      title: "Update",
      size: "xl",
      centered: true,
      autoShow: true,
      footer: { commands: ["close", "save"] },
      onAjaxContentLoaded: function (popup) {
        var form = popup.element.find("form");
        form.submit(function (event) {
          $(this).removeData("validator").removeData("unobtrusiveValidation");
          $.validator.unobtrusive.parse($(this));
          event.preventDefault();

          if (form.valid()) {
            var action = $(this).attr("action");
            var requestMethod = $(this).attr("method");
            var formData = $(this).serializeArray();
            var formDataObject = {};

            for (var i = 0; i < formData.length; i++) {
              formDataObject[formData[i].name] = formData[i].value;
            }

            $.ajax({
              type: requestMethod,
              url: action,
              data: formDataObject,
              success: function (data, textStatus, jQxhr) {
                if ($.isPlainObject(data)) {
                  ogBsTable.element.bootstrapTable("updateByUniqueId", {
                    id,
                    row: data.data,
                    replace: true,
                  });
                  popup.hide();
                  return;
                }
                $("#" + modalId)
                  .find(".modal-body")
                  .html(data);
              },
              error: function (jQxhr, textStatus, errorThrown) {
                console.log(errorThrown);
              },
            });
          }
        });

        if (ogBsTable.onPopupContentLoaded) {
          ogBsTable.onPopupContentLoaded(ogBsTable, popup, "create");
        }
      },
      onConfirmed: function (e, result, popup) {
        if (result === "confirmed") {
          var form = popup.element.find("form");
          form.submit();
        }
      },
    });
  }

  function operateDelete(ogBsTable, id) {
    $().ogPopup({
      ajax: ogBsTable.delete + "/" + id,
      name: "delete",
      title: "Delete",
      // size: "xl",
      centered: true,
      autoShow: true,
      footer: { commands: ["close", "save"] },
      onAjaxContentLoaded: function (popup) {
        var form = popup.element.find("form");
        form.submit(function (event) {
          $(this).removeData("validator").removeData("unobtrusiveValidation");
          $.validator.unobtrusive.parse($(this));
          event.preventDefault();

          if (form.valid()) {
            var action = $(this).attr("action");
            var requestMethod = $(this).attr("method");
            var formData = $(this).serializeArray();
            var formDataObject = {};

            for (var i = 0; i < formData.length; i++) {
              formDataObject[formData[i].name] = formData[i].value;
            }

            $.ajax({
              type: requestMethod,
              url: action,
              data: formDataObject,
              success: function (data, textStatus, jQxhr) {
                ogBsTable.element.bootstrapTable("removeByUniqueId", id);
                popup.hide();
              },
              error: function (jQxhr, textStatus, errorThrown) {
                console.log(errorThrown);
              },
            });
          }
        });
      },
      onConfirmed: function (e, result, popup) {
        if (result === "confirmed") {
          var form = popup.element.find("form");
          form.submit();
        }
      },
    });
  }
})(jQuery);
