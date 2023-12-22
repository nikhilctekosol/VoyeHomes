"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var testing_1 = require("@angular/core/testing");
var settlement_component_1 = require("./settlement.component");
describe('SettlementComponent', function () {
    var component;
    var fixture;
    beforeEach((0, testing_1.async)(function () {
        testing_1.TestBed.configureTestingModule({
            declarations: [settlement_component_1.SettlementComponent]
        })
            .compileComponents();
    }));
    beforeEach(function () {
        fixture = testing_1.TestBed.createComponent(settlement_component_1.SettlementComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });
    it('should create', function () {
        expect(component).toBeTruthy();
    });
});
//# sourceMappingURL=settlement.component.spec.js.map