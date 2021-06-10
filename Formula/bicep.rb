# to get the SHA256 checksum, replace the below command with the appropriate version:
# curl -Lf https://github.com/Azure/bicep/releases/download/v0.4.63/bicep-osx-x64 | shasum -a 256

class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.4.63"
  url "https://github.com/Azure/bicep/releases/download/v0.4.63/bicep-osx-x64"
  sha256 "ae3f091275c8f59e2ca4624e24a131d1b71d0790d29f1af8fde3b430e0803987"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
