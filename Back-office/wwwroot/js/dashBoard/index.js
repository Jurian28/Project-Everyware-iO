import { getConfig, upsert, changeSingularDataSetData } from '../chartHelper.js'

const mainContainer = document.getElementById("mainContainer");
const canvas = document.createElement("canvas")
const pieConfig = getConfig({title: 'testen'})

const chart = upsert(canvas, { config: pieConfig, labels: ["here", "Not Here", "free"], data: [1, 4, 5], datasetLabel: 'test' })

mainContainer.append(canvas)


setTimeout(() => {
    changeSingularDataSetData(chart, null, [200,100, 4, 5])
}, 10000);
