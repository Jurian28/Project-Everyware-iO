import { getConfig, upsert } from '../chartHelper.js'

const mainContainer = document.getElementById("mainContainer");
const canvas = document.createElement("canvas")
const pieConfig = getConfig({title: 'testen'})

upsert(canvas, { config: pieConfig, labels: ["here", "Not Here", "free"], data: [1, 4, 5], datasetLabel: 'test' })

mainContainer.append(canvas)