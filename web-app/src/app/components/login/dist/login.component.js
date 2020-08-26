"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
exports.__esModule = true;
exports.LoginComponent = void 0;
var core_1 = require("@angular/core");
var LoginComponent = /** @class */ (function () {
    function LoginComponent(loginSvr, titleService) {
        this.loginSvr = loginSvr;
        this.titleService = titleService;
        this.username = '';
        this.password = '';
    }
    LoginComponent.prototype.ngOnInit = function () {
        this.titleService.setTitle('登录');
    };
    /**
     * 登录
     */
    LoginComponent.prototype.login = function () {
        this.loginSvr.login(this.username, this.password);
    };
    /*
    * 按键触发
    * */
    LoginComponent.prototype.selectParaChange = function (e) {
        // tslint:disable-next-line: deprecation
        var keycode = window.event ? e.keyCode : e.which;
        if (keycode === 13) { // 回车键
            this.login();
        }
    };
    LoginComponent = __decorate([
        core_1.Component({
            selector: 'app-login',
            templateUrl: './login.component.html',
            styleUrls: ['./login.component.less']
        })
    ], LoginComponent);
    return LoginComponent;
}());
exports.LoginComponent = LoginComponent;
