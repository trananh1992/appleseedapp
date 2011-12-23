﻿/// <reference path="/Scripts/jquery-1.5-vsdoc.js" />




function scheduleUpdatePackage(packageId, schedule, source, version) {

    var actionurl = '/SelfUpdater/Updates/DelayedUpgrade';
    if (schedule == false) {
        actionurl = '/SelfUpdater/Updates/RemoveDelayedUpgrade';

        $('#schedule' + packageId).show();
        $('#schedule' + packageId).parents('tr').first().removeClass('ui-state-highlight ui-corner-all');

        $('#unschedule' + packageId).hide();

    } else {
        $('#schedule' + packageId).hide();
        $('#schedule' + packageId).parents('tr').first().addClass('ui-state-highlight ui-corner-all');

        $('#unschedule' + packageId).show();
    }

    $.ajax({
        url: actionurl,
        data: {
            packageId: packageId,
            source: source,
            version: version
        },
        dataType: 'json',
        timeout: 1200000,
        success: function (data) {

        },
        error: function (data) {
            trace(data);
            $('#upgradingDiv').dialog("close");
            alert("Communication error");
        }
    });

    return false;
}

function applyUpdates() {

    $('#upgradingDiv').dialog({
        modal: true,
        closeOnEscape: false,
        closeText: '',
        resizable: false,
        title: 'Upgrading site',
        open: function (event, ui) {
            $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
        }

    });

    $.post('/SelfUpdater/Updates/ApplyUpdates')
    .success(function () {
        $('<li>Applying updates...</li>').appendTo('#upgradingUl');

        var xhr;
        var reloading = false;
        var fn = function () {
            if (!reloading) {
                if (xhr && xhr.readystate != 4) {
                    xhr.abort();
                }
                xhr = $.post('/SelfUpdater/Updates/Status').success(function (data) {
                    if (data.online) {
                        $('<li>Reloading site...</li>').appendTo('#upgradingUl');
                        reloading = true;
                        window.location.reload();
                    }
                });
            }
        };

        var interval = setInterval(fn, 10000);
    })
    .error(function () {
        trace(data);
        $('#upgradingDiv').dialog("close");
        alert("Communication error");
    });


    return false;
}

function installPackage(packageId, source) {

    $('#installingDiv').dialog({
        modal: true,
        closeOnEscape: false,
        closeText: '',
        resizable: false,
        title: 'Install package',
        width: 550,
        height: 200,
        open: function (event, ui) {
            $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
        }

    });
    var status = false;
    $.ajax({
        url: "/SelfUpdater/Installation/InstallPackage",
        type: "POST",
        data: { packageId: packageId, source: source },
        timeout: 1200000,
        success: function (data) {
            //        var xhr;
            //        var reloading = false;
            //        var fn = function () {
            //            if (!reloading) {
            //                if (xhr && xhr.readystate != 4) {
            //                    xhr.abort();
            //                }
            //                xhr = $.post('/SelfUpdater/Updates/Status').success(function (data) {
            //                    if (data.online) {
            //                        $('<li>Reloading site...</li>').appendTo('#installingUl');
            //                        reloading = true;
            //                        window.location.reload();
            //                    }
            //                });
            //            }
            //        };

            //        var interval = setInterval(fn, 10000);     

            if (data.NugetLog != null && data.NugetLog != '') {
                $('#upgradingUl').html(data.NugetLog);
            }

            $.ajax({
                url: "/SelfUpdater/Updates/RestartSite",
                type: "POST",
                success: function (data) {
                    clearInterval(myinterval);
                },
                error: function () {

                }
            });

            var timeOutStatus = false;

            var interval = setInterval(function () {

                $.ajax({
                    url: "/SelfUpdater/Updates/Status",
                    type: "POST",
                    timeout: 200,
                    success: function (data) {

                        if (data) {


                            window.location = window.location.href;
                            clearInterval(interval);
                        }
                    },
                    error: function () {
                        if (!timeOutStatus) {
                            timeOutStatus = true;
                            $('#installingUl').append('<li>Reloading site...</li>')
                        }
                    }
                });
            }, 5000);
        },
        error: function () {
            trace(data);
            $('#installingDiv').dialog("close");
            alert("Communication error");
            clearInterval(myinterval);
        }
    });

    var myinterval = setInterval(function () {

        $.ajax({
            url: "/SelfUpdater/Updates/NugetStatus",
            type: "POST",
            async: false,
            success: function (data) {
                if (data != null && data != '') {
                    $('#installingUl').html(data);
                }
            }
        });
        if (status) {
            clearInterval(myinterval);
        }
    }, 2000);


    return false;

}

function updatePackage(packageId, source) {

    $('#upgradingDiv').dialog({
        modal: true,
        closeOnEscape: false,
        closeText: '',
        resizable: false,
        title: 'Update package',
        width: 550,
        height: 200,
        open: function (event, ui) {
            $(this).closest('.ui-dialog').find('.ui-dialog-titlebar-close').hide();
        }

    });


    $.ajax({
        url: '/SelfUpdater/Updates/Upgrade',
        type: "POST",
        data: { packageId: packageId, source: source },
        timeout: 1200000,
        success: function (data) {
            //        var xhr;
            //        var reloading = false;
            //        var fn = function () {
            //            if (!reloading) {
            //                if (xhr && xhr.readystate != 4) {
            //                    xhr.abort();
            //                }
            //                xhr = $.post('/SelfUpdater/Updates/Status').success(function (data) {
            //                    if (data.online) {
            //                        $('<li>Reloading site...</li>').appendTo('#installingUl');
            //                        reloading = true;
            //                        window.location.reload();
            //                    }
            //                });
            //            }
            //        };

            //        var interval = setInterval(fn, 10000);

            if (data.updated) {

                if (data.NugetLog != null && data.NugetLog != '') {
                    $('#upgradingUl').html(data.NugetLog);
                }


                $.ajax({
                    url: "/SelfUpdater/Updates/RestartSite",
                    type: "POST",
                    success: function (data) {
                        clearInterval(myinterval);
                    },
                    error: function () {

                    }
                });

                var timeOutStatus = false;

                var interval = setInterval(function () {

                    $.ajax({
                        url: "/SelfUpdater/Updates/Status",
                        type: "POST",
                        timeout: 200,
                        success: function (data) {

                            if (data) {


                                window.location = window.location.href;
                                clearInterval(interval);
                            }
                        },
                        error: function () {
                            if (!timeOutStatus) {
                                timeOutStatus = true;
                                $('#upgradingUl').append('<li>Reloading site...</li>');
                            }


                        }
                    });
                }, 5000);
            }
            else {
                clearInterval(myinterval);
                $('#upgradingUl').append('<li>There has been an error upgrading the package.</li>');
                $('#upgradingUl').append('<li>Please try again later.</li>')
            }
        },
        error: function () {
            trace(data);
            $('#upgradingDiv').dialog("close");
            alert("Communication error");
        }
    });


    var myinterval = setInterval(function () {

        $.ajax({
            url: "/SelfUpdater/Updates/NugetStatus",
            type: "POST",
            async: false,
            success: function (data) {
                if (data != null && data != '') {
                    $('#upgradingUl').html(data);
                }
            }
        });
        if (status) {
            clearInterval(myinterval);
        }
    }, 2000);

    return false;

}

