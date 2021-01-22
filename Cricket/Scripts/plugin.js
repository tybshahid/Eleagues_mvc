


$(function () {
    $.fn.Validator = function (options) {
        var $this = this;
        count = 0;
        var msgs = {
            required: "This field is required",
            maxLen: "No more than ",
            num: "Please fill a valid number",
            minLen: "Character length should be more than "
        };
        if (options == "validate") {//validate method
            var parent, el, msg, value, max, type, elType;

            $this.each(function () {
                el = $(this);
                msg = msgs.required;
                elType = el.prop('tagName');

                if (elType == "SELECT") {
                    value = el.val();
                }
                else {
                    value = el.val() || el.html();
                }

                max = el.attr('data-len');
                min = el.attr('data-min');
                type = el.attr('type');

                if (el.attr('data-req') == "true" && value == "") {

                    if (type == "number") {
                        msg = msgs.num;
                    }
                    el.parent().find('.msg-box')
                               .addClass('active')
                               .text(msg);
                    el.addClass('error-border');
                    count = -1;
                }
                else {
                    if (type == "number") {
                        max = parseInt(el.attr('max'));
                        min = parseInt(el.attr('min'));

                        if (value > max) {

                            msg = "Value should be less than " + max;
                            el.parent().find('.msg-box')
                               .addClass('active')
                               .text(msg);
                            el.addClass('error-border');
                            count = -1;
                        }
                        else if (value < min) {
                            msg = "Value should be greater than " + min;
                            el.parent().find('.msg-box')
                               .addClass('active')
                               .text(msg);
                            el.addClass('error-border');
                            count = -1;
                        }
                    }
                    else {
                        if (value.length > max) {

                            msg = msgs.maxLen + max + " characters";
                            el.parent().find('.msg-box')
                               .addClass('active')
                               .text(msg);
                            el.addClass('error-border');
                            count = -1;
                        }
                        else if (value.length < min) {
                            msg = msgs.minLen + min;
                            el.parent().find('.msg-box')
                               .addClass('active')
                               .text(msg);
                            el.addClass('error-border');
                            count = -1;
                        }
                    }
                }
            });

            setTimeout(function () {
                $this.parent().find('.msg-box').removeClass('active');
            }, 2500); //remove in 2.5 secs
            return count;
        }
        else if (typeof (options) != "undefined") { //initialization
            var settings = $.extend({
                bgColor: "#f05050",
                msg: "Please fill a valid entry",
                min: 0
            }, options);



            var div = $('<div class="msg-box">' + settings.msg + '</div>');
            div.css({
                "background-color": settings.bgColor,
                color: "#fff"
            });
            $this.after(div);

            $this.attr({
                'data-req': settings.required
            });

            $this.attr({
                'data-len': settings.maxLength
            });

            $this.attr({
                'data-min': settings.minLength
            });

            $this.on('input', function () {
                $(this).removeClass('error-border');
            });
        }
    };
});


function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}