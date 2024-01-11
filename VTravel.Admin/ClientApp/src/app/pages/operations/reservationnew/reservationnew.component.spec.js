"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var reservationnew_component_1 = require("./reservationnew.component");
describe('ReservationnewComponent', function () {
    var component;
    var fixture;
    beforeEach((0, testing_1.async)(function () {
        testing_1.TestBed.configureTestingModule({
            declarations: [reservationnew_component_1.ReservationnewComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = testing_1.TestBed.createComponent(reservationnew_component_1.ReservationnewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=reservationnew.component.spec.js.map