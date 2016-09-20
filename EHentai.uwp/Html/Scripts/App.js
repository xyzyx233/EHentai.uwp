var ImageList = new Array();
var app;
var scope;
var imageWidth;//单个图片元素的宽度
var loadSize = 700;//加载下一页的距离



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