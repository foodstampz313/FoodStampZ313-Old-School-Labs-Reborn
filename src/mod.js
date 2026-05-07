
const fs = require("fs");
const path = require("path");

class Mod {
    postDBLoad(container) {
        const logger = container.resolve("WinstonLogger");
        const databaseServer = container.resolve("DatabaseServer");
        const tables = databaseServer.getTables();
        const items = tables.templates.items;
        const handbook = tables.templates.handbook.Items;
        const prices = tables.templates.prices;

        const cfg = JSON.parse(fs.readFileSync(path.resolve(__dirname, "../config/config.json"), "utf8"));
        const prefix = `[${cfg.loggerPrefix}]`;

        let changed = 0;

        for (const [id, data] of Object.entries(cfg.keycards)) {
            const item = items[id];
            if (!item) {
                logger.warning(`${prefix} Missing ${data.name}`);
                continue;
            }

            if (cfg.enableUnlimitedUses) {
                item._props.MaximumNumberOfUsage = 0;
            }

            if (cfg.enablePriceChanges) {
                prices[id] = data.price;
                const hbEntry = handbook.find(x => x.Id === id);
                if (hbEntry) hbEntry.Price = data.price;
            }

            changed++;
            logger.info(`${prefix} Loaded ${data.name}`);
        }

        logger.success(`${prefix} Successfully modified ${changed} Labs keycards.`);
    }
}

module.exports = { mod: new Mod() };
