﻿var ImageList = new Array();
var app;
var scope;

function reload() {
    ImageList.splice(0, ImageList.length);
    scope.$apply();
}

//添加项目
function AddImage(image) {
    if (image) {
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