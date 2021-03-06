﻿$.widget("ui.testcase", {
    version: "1.0.0",
    _create: function () {
        var drawing = false;
        var startPoint;
        var $label = this.element.find(".label");
        var that = this;
        var currSquare;
        var locked = false;
        var scope = "local";
        this.element.find("[data-testcase-element='scope']").on("click", ".btn", function() {
            $(this).siblings().removeClass("active");
            $(this).addClass("active");
            scope = $(this).data("testcase-element");
        });

        var $board = this.element.find("[data-testcase-element='board']");
        function getNumber(percentageText) {
            return parseFloat(percentageText.replace("%", ""));
        }
        this.element.on("click", "[data-testcase-element='save']", function (e) {
            e.preventDefault();
            if (locked) {
                return;
            }
            var maxWidth = $board.width();
            var maxHeight = $board.height();
            var localSpots = [];
            var categorySpots = [];
            var globalSpots = [];
            var screenshotDimensions = that._getScreenshotDimensions();
            $board.find(".blind").each(function () {
                var $this = $(this);
                var spot ={
                    left: getNumber($this.css("left")) / maxWidth * screenshotDimensions.width,
                    top: getNumber($this.css("top")) / maxHeight * screenshotDimensions.height,
                    width: getNumber($this.css("width")) / maxWidth * screenshotDimensions.width,
                    height: getNumber($this.css("height")) / maxHeight * screenshotDimensions.height
                };
                if ($this.is(".local")) {
                    localSpots.push(spot);
                }else if ($this.is(".category")) {
                    categorySpots.push(spot);
                }else {
                    globalSpots.push(spot);
                }
                
            });
            locked = true;
            if ($(this).is(".local")) {
                $.postJSON(that.options.actionSaveLocal, {
                    BrowserPatternId: that.options.id,
                    TestCaseId: that.options.caseId,
                    LocalBlindRegions: localSpots
                }).done(function() {
                    locked = false;
                });
            }else  if ($(this).is(".category")) {
                $.postJSON(that.options.actionSaveCategory, {
                    BlindRegions: categorySpots,
                    BrowserName: that.options.browser,
                    TestCaseId: that.options.caseId
                }).done(function () {
                    locked = false;
                });
            } else {
                $.postJSON(that.options.actionSaveGlobal, {
                    BlindRegions: globalSpots,
                    BrowserName: that.options.browser,
                    TestCaseId: that.options.caseId
                }).done(function () {
                    locked = false;
                });
            }
            
        });
        $board.on("keyup", ".blind", function (e) {
            if (e.keyCode == 46) {
                $board.find(".blind.active").remove();
            }
        });

        $board.on("focus", ".blind", function (e) {
            $(this).addClass("active");
        });

        $board.on("blur", ".blind.active", function (e) {
            $(this).removeClass("active");
        });

        $board.on("mousedown", ".blind", function (e) {
            e.stopPropagation();
        });
        var index = 1;

        function addSquareToBoard(data, type) {
            var screenshotDimensions = that._getScreenshotDimensions();
            var square = $("<div></div>", { "class": "blind "+type, tabindex: index++ });
            $board.append(square);
            square.css({
                left: data.Left / screenshotDimensions.width * 100.0 + "%",
                top: data.Top / screenshotDimensions.height * 100 + "%",
                width: data.Width / screenshotDimensions.width * 100.0 + "%",
                height: data.Height / screenshotDimensions.height * 100.0 + "%"
            });
        }

        this.element.find(".screenshot").on("load", function() {
            that.options.localRegions.forEach(function (data) {
                addSquareToBoard(data, "local");
            });

            that.options.categoryRegions.forEach(function (data) {
                addSquareToBoard(data, "category");
            });

            that.options.globalRegions.forEach(function (data) {
                addSquareToBoard(data, "global");
            });
        });
        

        $board.on("mousedown", function (e) {
            drawing = true;
            startPoint = getOffset(e);
            currSquare = $("<div></div>",{"class":"blind "+scope, tabindex:index++});
            $board.append(currSquare);
        });
        $board.on("mouseup", function () {
            if (drawing) {
                drawing = false;
                if (currSquare.width() < 5 || currSquare.height() < 5) {
                    currSquare.remove();
                }
                currSquare.focus();
                currSquare = null;
            }
        });

        function getOffset(e) {
            var parentOffset = $board.offset();
            return {
                x: e.pageX - parentOffset.left,
                y: e.pageY - parentOffset.top
            };
        }

        $board.on("mousemove", function (e) {
            var offset = getOffset(e);

            $label.text("X:" + offset.x + " Y:" + offset.y);

            if (drawing) {
                var maxWidth = $board.width();
                var maxHeight = $board.height();
                currSquare.css({
                    left: Math.min(startPoint.x, offset.x) / maxWidth *100 + "%",
                    top: Math.min(startPoint.y, offset.y) / maxHeight * 100 + "%",
                    width: Math.abs(offset.x - startPoint.x) / maxWidth * 100 + "%",
                    height: Math.abs(offset.y - startPoint.y) / maxHeight * 100 + "%"
                });
            }
        });
    },
    _getScreenshotDimensions: function() {
        var screenshot = this.element.find(".screenshot").get(0);
        return {
            width: screenshot.naturalWidth,
            height: screenshot.naturalHeight
        };
    }
});