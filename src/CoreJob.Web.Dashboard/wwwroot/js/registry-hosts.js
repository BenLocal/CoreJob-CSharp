var $jobDataMapRowCounter = 0;

function registryHostsItem() {
    return this.each(function () {

        var row = $(this);
        var table = row.closest('table');

        function nameChanged(e) {

            if (e.target.value === "")
                return;

            var lastRow = table.find('>tbody>tr:last');

            if (row.is(lastRow)) {
                // add new row only if we editing the last row
                addItem();
            }
        }

        function addItem() {
            var elm = $registryHostsRowTemplate.clone()
                .appendTo(table.find('>tbody'))
                .registryHostsItem();
        }

        function deleteItem() {
            var re = function () {
                if (visibleRows.length === 1) {
                    addItem(); // add empty row
                }

                row.remove();
                reIndex();
            } 

            var visibleRows = table.find('>tbody>tr');
            var id = row.data("row-id");
            if (id > 0) {
                // server delete
                $('#dimmer').dimmer('show');
                $.ajax({
                    type: 'GET',
                    url: "/executer/deleteHost/" + id,
                    contentType: 'application/json',
                    success: function () {
                        re();
                        $('#dimmer').dimmer('hide');
                    },
                    error: function (e) {
                        $('#dimmer').dimmer('hide');
                        prependErrorMessage(e, msgParent);
                    }
                });
            } else {
                re();
            }
        }

        function cloneItem() {
            var elm = row.clone()
                .insertBefore(row)
                .registryHostsItem();

            elm.find('.accept-error.error').each(function () { $(this).removeClass('error'); });
        }

        function setUniqueInputNames() {
            var rowIndex = row.data('row-index');
            $(this).find('input').each(function () {
                var nameTemp = $(this).data('name-temp');
                var nameBase = $(this).data('name-base');
                $(this).attr('name', nameBase + '[' + rowIndex + '].' + nameTemp); // ensure every input has unique name
            });
        }

        function initValueElement() {
            var valueCol = row.find('.value-col');
            setUniqueInputNames.call(valueCol);
            valueCol.wrapInner("<div class='value-container'></div>");
        }

        function reIndex() {
            $jobDataMapRowCounter = 0;
            $('#registry_hosts>tbody>tr').each(function () {
                var row = $(this);
                row.data('row-index', $jobDataMapRowCounter);
                $(this).find('input').each(function () {
                    var nameTemp = $(this).data('name-temp');
                    var nameBase = $(this).data('name-base');
                    $(this).attr('name', nameBase + '[' + $jobDataMapRowCounter + '].' + nameTemp); // ensure every input has unique name
                });
                $jobDataMapRowCounter++;
            });
        }

        // init components
        row.data('row-index', $jobDataMapRowCounter);
        $jobDataMapRowCounter++;
        setUniqueInputNames.call(row.find('.name-col, .type-col'));
        initValueElement();

        // event handlers
        row.find('.name-col input').on('input', nameChanged);
        row.find('.delete-row').click(deleteItem);
        row.find('.copy-row').click(cloneItem);
    });
}

var $registryHostsRowTemplate;

$(function() {
    $registryHostsRowTemplate = $('#registry_hosts>tbody>tr.template').detach().removeClass('template');
    $.fn.registryHostsItem = registryHostsItem;
    $('#registry_hosts>tbody>tr').registryHostsItem();
});
