#!/bin/bash

readonly DIR="${1:-???}"
RUNTIME="${2:-net8.0}"

if [ "${RUNTIME}" == "windows" ];then
    RUNTIME="net8.0-windows10.0.22621.0";
elif [ "${RUNTIME}" == "android" ];then
    RUNTIME="net8.0-android";
fi

readonly RUNTIME

echo "Runtime: ${RUNTIME}"

function die()
{
    local message="${1:-???}"
    echo "Exiting: ${message}"

    exit 1
}

BUILD_RESULT_DIR="???"

readonly OUTPUT_DIR="${PWD}/assemble-temp"

readonly TARGET_DIR="${PWD}/target"

function createDirOrDie()
{
    local path="${1}"

    mkdir -vp "${path}" || die "Unable to create directory '${path}'"
}

function init()
{
    if [ ! -d "${PWD}/${DIR}" ];then
        die "'${PWD}/${DIR}' is not a valid directory"
    fi

    rm -r "${OUTPUT_DIR}"
    rm -r "${TARGET_DIR}"

    createDirOrDie "${OUTPUT_DIR}"
    createDirOrDie "${OUTPUT_DIR}/zh"

    createDirOrDie "${TARGET_DIR}"

    export BUILD_RESULT_DIR="${PWD}/${DIR}/bin/Release/${RUNTIME}"
    readonly BUILD_RESULT_DIR

    echo "Final directory is '${BUILD_RESULT_DIR}'"
}

function copy()
{
    local fileName="${1:-???}"

    if [ "${fileName}" == "???" ];then
        die "Invalid usage! No file input."
    fi

    if ! cp "${BUILD_RESULT_DIR}/${fileName}" "${OUTPUT_DIR}/${fileName}";then
        echo "Failed copy ${fileName}";
        return 1;
    fi

    echo "Done! '${BUILD_RESULT_DIR}/${fileName}' --> '${OUTPUT_DIR}/${fileName}'"
}

function main()
{
    copy "LLin.OSIntegrations.dll"
    copy "Tmds.DBus.Protocol.dll"
    copy "M.Resources.dll"
    copy "osu.Game.Rulesets.Hikariii.dll"
    copy "zh/M.Resources.resources.dll"

    copy "Microsoft.Windows.SDK.NET.dll" || echo "Ignoring WindowsSDK..."
    copy "WinRT.Runtime.dll" || echo "Ignoring WinRT..."

    cp "${PWD}/README.md" "${OUTPUT_DIR}" || echo "Unable to copy README.md, ignoring..."

    local lastPWD="${PWD}"

    cd "${OUTPUT_DIR}" || die "Why?!"

    zip -r "${TARGET_DIR}/Hikariii.${RUNTIME}.zip" .
}

init;
main;