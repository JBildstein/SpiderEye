"use-strict";

const gulp = require('gulp');
const { series, parallel } = require('gulp');
const gulpTslint = require("gulp-tslint");
const tsc = require("gulp-typescript");
const del = require("del");

function clean() {
    return del([
        "bin",
        "lib",
        "es",
        "dts"
    ]);
}

function lint() {
    return gulp.src(["src/**/*.ts"])
        .pipe(gulpTslint({ formatter: "stylish" }))
        .pipe(gulpTslint.report());
}

function buildLib() {
    var project = tsc.createProject("tsconfig.json", { module: "commonjs" });
    return gulp.src(["src/**/*"])
        .pipe(project())
        .js.pipe(gulp.dest("lib/"));
}

function buildES() {
    var project = tsc.createProject("tsconfig.json", { module: "es6" });
    return gulp.src(["src/**/*"])
        .pipe(project())
        .js.pipe(gulp.dest("es/"));
}

function buildTypings() {
    var project = tsc.createProject("tsconfig.json", { declaration: true });
    return gulp.src(["src/**/*"])
        .pipe(project())
        .dts.pipe(gulp.dest("dts/"));
}

function copyTypings() {
    return gulp.src(["src/**/*.d.ts"])
        .pipe(gulp.dest("dts/"));
}

const build = parallel(buildLib, buildES, buildTypings);
exports.default = series(clean, lint, build, copyTypings);
