import { Accordion } from "@vscode-bicep/ui-components";
import { ResourceTypeList } from "./components/resource-type-list";
import { ResourceProviderHeader } from "./components/resource-provider-header";
import { useResourceTypeCatalog } from "./hooks/use-resource-type-catalog";

function App() {
  const resourceTypeCatalog = useResourceTypeCatalog();

  return (
    <section>
      <Accordion>
        {Object.entries(resourceTypeCatalog).map(
          ([resourceProvider, resourceTypes], i) => (
            <Accordion.Item key={i}>
              <Accordion.ItemCollapse>
                <ResourceProviderHeader resourceProvider={resourceProvider} />
              </Accordion.ItemCollapse>
              <Accordion.ItemContent>
                <ResourceTypeList
                  resourceProvider={resourceProvider}
                  resourceTypes={resourceTypes}
                />
              </Accordion.ItemContent>
            </Accordion.Item>
          ),
        )}
      </Accordion>
    </section>
  );
}

export default App;
