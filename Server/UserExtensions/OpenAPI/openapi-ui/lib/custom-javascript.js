var links = document.getElementsByTagName("link");

for (var i in links) {
    if (links[i].rel === "icon") {
        if (links[i].sizes == "32x32") {
            links[i].href = "/openapi-ui/favicon-32x32.png";
        }
        else {
            links[i].href = "/openapi-ui/favicon-16x16.png";
        }        
    }
}