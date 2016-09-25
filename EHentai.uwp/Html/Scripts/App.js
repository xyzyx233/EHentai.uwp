﻿var ImageList = new Array();
var app;
var scope;
var imageWidth;//单个图片元素的宽度
var loadSize = 700;//加载下一页的距离
var errorImage = "data:image/jpeg;base64,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";

/**
 * 初始化angular设置
 */
app = angular.module('myApp', ['ngMaterial', 'ngAnimate']);

//设置图标
app.run(function ($templateCache) {
    angular.forEach(assetMap, function (value, key) {
        $templateCache.put(key, value);
    });
});

//图片相关事件
app.directive('imageonload', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.addClass("ng-hide-remove");
            var fn = $parse(attrs.imageonload);
            element.bind('load', function (event) {
                element.addClass("ng-hide-add");
                //scope.$apply(attrs.imageonload);
                scope.$apply(function () {
                    fn(scope, { $event: event });
                });
            });
            element.bind('error', function () {
                //scope.$apply(attrs.imageonload);
                attrs.$$element[0].src = errorImage;
            });
        }
    };
});

//设置主题
app.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('myTheme')
        .primaryPalette('grey')
        .accentPalette('blue')
        .warnPalette('red');
    $mdThemingProvider.alwaysWatchTheme(true);
});



//刷新当前页面的数据
function reload() {
    scope.isLoading = true;
    ImageList.splice(0, ImageList.length);
    scope.$apply();
}

//添加项目
function AddImage(image) {
    if (image) {
        if (scope.isLoading) {
            scope.isLoading = false;
        }
        var data = JSON.parse(image);
        //防止重复添加
        for (var i = 0; i < ImageList.length; i++) {
            var img = ImageList[i];
            if (data.CacheName == img.CacheName) {
                return;
            }
        }
        ImageList.push(data);
        scope.$apply();
    }
}

//批量添加
function AddImages(images) {
    if (images) {
        if (scope.isLoading) {
            scope.isLoading = false;
        }
        var datas = JSON.parse(images);

        for (var i = 0; i < datas.length; i++) {
            var data = datas[i];
            //防止重复添加
            for (var j = 0; j < ImageList.length; j++) {
                var img = ImageList[j];
                if (data.CacheName == img.CacheName) {
                    return;
                }
            }
            ImageList.push(data);
        }
        scope.$apply();
    }
}

//设置图片路径
function SetSrc(image) {
    if (image) {
        var data = JSON.parse(image);
        for (var i = 0; i < ImageList.length; i++) {
            var img = ImageList[i];
            if (data.CacheName == img.CacheName && !img.Src) {
                img.Visibility = true;
                img.Src = data.Src;
                scope.$apply();
                return;
            }
        }
    }
}

//内容居中显示
function ContentCenter() {
    var width = Math.floor($("#content").width() / imageWidth) * imageWidth;
    $("#divImageList").width(width);
}

window.onresize = function () {
    ContentCenter();
}

$(function () {
    ContentCenter();
    documentDorp();
});

//滚动翻页
function imageScroll() {
    var scrollTop = $('#content').scrollTop();
    var contentHeight = $('#divImageList').height();//内容高度
    var windowHeight = $('#content').height();//可视高度
    if (scrollTop + windowHeight > contentHeight - loadSize) {
        var data = { method: 'Scroll', data: scrollTop };
        window.external.notify(JSON.stringify(data));
    }
};

//获取图片的Base64
function getBase64Image(img) {

    var canvas = document.createElement("canvas");
    canvas.width = img.width;
    canvas.height = img.height;

    var ctx = canvas.getContext("2d");
    ctx.drawImage(img, 0, 0);


    var dataUrl = canvas.toDataURL();

    return dataUrl.replace(/^data:image\/(png|jpg|gif|bmp);base64,/, "");
}
//将图片缓存到APP
function cacheImage(img, model) {
    model.ImageUrl = getBase64Image(img);
    var data = { method: 'CacheImage', data: model };
    window.external.notify(JSON.stringify(data));
}


/**
 * 拖动图片新窗口显示
 */
function documentDorp() {
    //阻止浏览器默认行。 
    $(document).on({
        dragleave: function (e) {    //拖离 
            e.preventDefault();
        },
        drop: function (e) {  //拖后放 
            e.preventDefault();
            dorp(e.originalEvent);
        },
        dragenter: function (e) {    //拖进 
            e.preventDefault();
        },
        dragover: function (e) {    //拖来拖去 
            e.preventDefault();
        }
    });
}

function dorp(e) {
    var files = e.dataTransfer.files; //获取文件对象 
    //检测是否是拖拽文件到页面的操作 
    if (files.length == 0) {
        return false;
    }

    //遍历所有文件
    for (var i = 0; i < files.length; i++) {
        readerImage(files[i]);
    }
}

function readerImage(file) {
    //检测文件是不是图片 
    if (file.type.match(/image*/)) {
        //读取图片信息
        var reader = new FileReader();
        //创建一个Image用来获取图片的尺寸
        var img = new Image();

        $(reader).bind('load', img, function (event) {
            event.data.src = this.result;
        });

        //将图片转为base64
        reader.readAsDataURL(file);

      
        //设置图片的加载事件
        img.onload = function () {
            var data = { method: 'ToImageView', data: { ImgBase64: this.src, Width: this.width, Height: this.height } };

            //$('body').append(img.outerHTML);
            window.external.notify(JSON.stringify(data));
        }
    }
}
