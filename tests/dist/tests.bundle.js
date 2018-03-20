(function(l, i, v, e) { v = l.createElement(i); v.async = 1; v.src = '//' + (location.host || 'localhost').split(':')[0] + ':35729/livereload.js?snipver=1'; e = l.getElementsByTagName(i)[0]; e.parentNode.insertBefore(v, e)})(document, 'script');
(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory() :
	typeof define === 'function' && define.amd ? define(factory) :
	(factory());
}(this, (function () { 'use strict';

function tests(_arg1) {
    QUnit.module("HtmlConverter");
    QUnit.test("simple tag", function (test) {
        (function (arg00, arg10) {
            test.equal(arg00, arg10);
        })("", "div [ ] [ ]");
    });
}

tests();

})));
//# sourceMappingURL=tests.bundle.js.map
