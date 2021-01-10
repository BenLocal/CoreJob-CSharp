// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function initDimmer() {
    return $('#dimmer').dimmer({ closable: false, duration: 100, opacity: 1 });
}

function doPost(options) {
    options = options || {};

    $('#dimmer').dimmer('show');
    $.ajax({
        type: 'POST',
        url: options.url,
        data: JSON.stringify(options.data || {}),
        contentType: options.contentType || 'application/json',
        success: function (res) {
            if (res.cod == "ok") {
                options.success && options.success(res.data);
            } else {
                prependErrorMessage(res.msg, $('#msg-panel'));
            }
            $('#dimmer').dimmer('hide');
        },
        error: function (e) {
            options.error && options.error(e);

            $('#dimmer').dimmer('hide');
            prependErrorMessage(e, $('#msg-panel'));
        }
    });
}

function doGet(options) {
    options = options || {};

    $('#dimmer').dimmer('show');
    $.ajax({
        type: 'Get',
        url: options.url,
        contentType: options.contentType || 'application/json',
        success: function (res) {
            if (res.cod == "ok") {
                options.success && options.success(res.data);
            } else {
                prependErrorMessage(res.msg, $('#msg-panel'));
            }
            $('#dimmer').dimmer('hide');
        },
        error: function (e) {
            options.error && options.error(e);

            $('#dimmer').dimmer('hide');
            prependErrorMessage(e, $('#msg-panel'));
        }
    });
}

function getErrorMessage(e) {
    if (typeof e == 'string') {
        const msg =
            '<div class="ui negative message error-context">' +
            '<i class="close icon"></i>' +
            '<div class="header">错误</div><p>' +
            e +
            '</p></div>';

        return msg;
    }

    var statusNum = '';
    var statusText = e.statusText;

    if (e.responseJSON && e.responseJSON.ExceptionMessage)
        statusText = e.responseJSON.ExceptionMessage;

    if (e.status > 0)
        statusNum = ' (' + e.status + ')';

    const msg =
        '<div class="ui negative message error-context">' +
        '<i class="close icon"></i>' +
        '<div class="header">An error occured' + statusNum +
        '</div><p>' + statusText + '</p></div>';

    return msg;
}

function initErrorMessage(errorElements) {
    $(this)
        .transition('fade in')
        .find('.close')
        .on('click', function () {
            if (errorElements) {
                errorElements.removeClass('error');
            }
            $(this).closest('.message').transition('fade');
        });

    return $(this);
}

function prependErrorMessage(e, parent) {

    parent.prepend(initErrorMessage.call($(getErrorMessage(e))));
}

function clearErrorMessage(parent) {
    parent.find(".error-context").remove();
}

function deleteItem(key, msgParent, delUrl, redirUrl) {

    $('#delete-dialog')
        .modal({
            duration: 250,
            onApprove: function () {

                $('#dimmer').dimmer('show');

                $.ajax({
                    type: 'POST',
                    url: delUrl,
                    data: JSON.stringify(key),
                    contentType: 'application/json',
                    cache: false,
                    success: function () {
                        document.location = redirUrl;
                    },
                    error: function (e) {
                        $('#dimmer').dimmer('hide');
                        prependErrorMessage(e, msgParent);
                    }
                });

            }
        })
        .modal('show');
}

function processValidationResponse(data, segmentRoot) {

    $('.accept-error.error').each(function () { $(this).removeClass('error'); });

    function cleanup() {
        $(this)
            .off('focusin', cleanup)
            .removeClass('error');
        $(this).popup("remove popup");
    }

    if (data.suc === false) {

        for (i = 0; i < data.fs.length; i++) {
            var
                err = data.fs[i],
                field = err.field,
                errElm;

            if (field.startsWith('VM.')) {
                var fieldName = field.substring(3);
                if (segmentRoot && (err.SegmentIndex > 0 || err.SegmentIndex === 0)) {
                    errElm = $('.segment:nth-child(' + (err.SegmentIndex + 1) + ') ' + "[name='" + fieldName + "']", segmentRoot);
                } else {
                    errElm = $("[name='" + fieldName + "']");
                }

                if (err.FieldIndex > 0 || err.FieldIndex === 0)
                    errElm = $(errElm[err.FieldIndex]);

                if (!errElm.hasClass('accept-error'))
                    errElm = errElm.closest('.accept-error');
            }

            errElm.addClass('error');
            errElm.on('focusin', cleanup);

            errElm.popup({
                content: err.reason,
                hoverable: true,
                position: 'top left',
                variation: 'inverted',
                distanceAway: -20
            });
        }
    }

    return data.suc;
}

function Pagination(obj) {
    this.id = obj.id;  //div id
    this.url = obj.url;
    this.pageSize = obj.pageSize;
    this.pageNum = 1; //current page number
    this.total = 0; //total count
    this.totalPage = 0;
    this.barSize = obj.barSize; //分页工具条上展现的页码数
    this.numPoint = 1;
    this.data = obj.data;
    this.success = obj.success;
    this.error = obj.error;
    this.div = null;
    this.init();
}
Pagination.prototype.init = function () {
    if (this.data == undefined) {
        this.data = {}
    }
    this.div = $('#' + this.id);
    this.fetchData(this.pageNum);
};

Pagination.prototype.fetchData = function (pageNum) {
    this.data.pageNum = pageNum;
    this.data.pageSize = this.pageSize;
    var that = this;
    $.ajax({
        url: that.url,
        data: that.data,
        type: 'post',
        dataType: 'json',
        success: function (res) {
            if (res.cod != "ok") {
                return;
            }
            var data = res.data;
            if (data.total == undefined) {
                that.success(data.rows);
                return;
            }
            that.total = data.total;
            var tmp = that.total % that.pageSize;
            var num = Math.floor(that.total / that.pageSize);
            that.totalPage = tmp == 0 ? num : num + 1;
            that.showUI(pageNum);
            that.success(data.rows);
        },
        error: function (msg) {
            that.error(msg);
        }
    })
};
Pagination.prototype.showUI = function (pageNum) {
    var that = this;
    this.div.empty();
    var currentBarSize = this.totalPage - (pageNum - 1) * this.barSize;
    currentBarSize = currentBarSize > this.barSize ? this.barSize : currentBarSize;
    this.div.append('<a class="item"><i class="left arrow icon"></i> 上一页</a>');

    if (this.totalPage - pageNum < this.barSize + 1) {
        for (var i = 0; i < this.barSize; i++) {
            var currrent = this.totalPage - this.barSize + 1 + i;
            if (currrent <= 0) {
                continue;
            }
            this.div.append('<a class="item">' + (this.totalPage - this.barSize + 1 + i) + '</a>');
        }
    } else {
        for (var i = 0; i < this.barSize; i++) {
            var currrent = pageNum + i;
            if (currrent > this.totalPage) {
                continue;
            }
            this.div.append('<a class="item">' + (pageNum + i) + '</a>');
        }
    }

    this.div.append('<a class="item"> 下一页 <i class="icon right arrow"></i></a>');

    var array = this.div.find('a');
    for (var j = 0; j < array.length; j++) {
        var current = $(array[j]);
        var n = parseInt(current.text().trim());
        pageNum == n ? current.addClass("active") : current.removeClass("active");
        if (j == 0) {
            pageNum == 1 ? current.addClass("disabled") : current.removeClass("disabled");
            current.click({ param: that }, that.previewPage);
        } else if (j == array.length - 1) {
            pageNum == this.totalPage ? current.addClass("disabled") : current.removeClass("disabled");
            current.click({ param: that }, that.nextPage)
        } else {
            current.click({ param: that }, function (event) {
                var p = event.data.param;
                var n = $(this).text().trim();
                p.fetchData(parseInt(n));
            })
        }
    }
};
Pagination.prototype.nextPage = function (event) {
    var p = event.data.param;
    var that = p.data;
    if (p.totalPage < that.pageNum + 1) {
        p.div.find('a').last().addClass("disabled");
        return;
    } else {
        p.div.find('a').last().removeClass("disabled");
    }
    that.pageNum++;
    p.fetchData(that.pageNum);
};
Pagination.prototype.previewPage = function (event) {
    var p = event.data.param;
    var that = p.data;
    if (that.pageNum - 1 <= 0) {
        p.div.find('a').first().addClass("disabled");
        return;
    } else {
        p.div.find('a').first().removeClass("disabled");
    }

    that.pageNum--;
    p.fetchData(that.pageNum);
};


function initCronLiveDescription(url, $cronInput, $cronDesc, $nextCronDates) {
    function describeCron() {
        $.ajax({
            type: 'POST',
            url: url,
            timeout: 5000,
            data: JSON.stringify({
                text: $cronInput.val()
            }),
            contentType: 'application/json',
            dataType: 'json',
            success: function (data) {
                $cronDesc.text(data.description);
                var nextHtml = data.next.join('<br>');
                if (nextHtml === '') $nextCronDates.hide(); else {
                    $nextCronDates.show();
                    $nextCronDates.popup({ html: '<div class="header">执行时间</div><div class="content">' + nextHtml + '</div>' });
                }
            },
            error: function (e) { 
                $cronDesc.text('发生错误.'); }
        });
    }
    var cronDescTimer;
    $cronInput.on('input', function (e) {
        window.clearTimeout(cronDescTimer);
        searchcronDescTimerTimer = window.setTimeout(function () {
            cronDescTimer = null;
            describeCron();
        }, 250);
    });

    describeCron();
}

