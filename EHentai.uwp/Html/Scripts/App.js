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

$(function () {
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
});

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
