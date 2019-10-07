import * as Express from 'express'
import { json, urlencoded } from 'body-parser'

import { buildDepTreeFromProjectFile } from 'dotnet-deps-parser'

let app = Express();
app.use(urlencoded({ extended: true }))
  .use(json());

app.get(
  '/ping',
  (req: Express.Request, res: Express.Response, next) => {
    (async () => {
      res = res.status(200).send();
    })().catch(
      (reason) => {
        console.log(reason)
        next(reason);
      })
  });

app.post(
  '/dependencies',
  (req: Express.Request, res: Express.Response, next) => {
    (async () => {
      let projectContents = req.body["contents"];
      if (projectContents === undefined) {
        console.error(req.body);
        res.status(404).send("Not found key 'contents' in your request body.");
        return;
      }
      let tree = await buildDepTreeFromProjectFile(projectContents, false);
      var x = Object.keys(tree.dependencies).map(function (key) {
        console.log(key);
        return tree.dependencies[key].name;
      });
      res.send({ "dependencies": x });
    })().catch(
      (reason) => {
        console.log(reason)
        next({ reason });
      })
  });

app.listen(8080);

